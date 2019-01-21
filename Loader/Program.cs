using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Features;
using OsmSharp;
using OsmSharp.Geo;
using OsmSharp.Geo.Streams.Features.Interpreted;
using OsmSharp.Streams;
using OsmSharp.Streams.Complete;
using OsmSharp.Tags;
using Persistency;
using Persistency.Entities;
using Persistency.Services;
using static Persistency.Persistency;

namespace Loader
{
    static class Program
    {
        static void Exit(IdentityResult result, IDbContextTransaction transaction)
        {

            foreach (var error in result.Errors)
            {
                Console.Error.WriteLine(error.Description);
            }

            transaction.Rollback();
            Console.Error.WriteLine("Exiting");
            Environment.Exit(1);
        }
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Filepath not given");
                Environment.Exit(1);
            }
            var amenities = new HashSet<string>
                {
                    "bar",
                    "bbq",
                    "biergarten",
                    "cafe",
                    "fast_food",
                    "food_court",
                    "ice_cream",
                    "pub",
                    "restaurant"
                };
            var types = new HashSet<OsmGeoType> { OsmGeoType.Node, OsmGeoType.Way };
            var configuration = new ConfigurationBuilder()
                .Add(
                    new MemoryConfigurationSource
                    {
                        InitialData = new[] { new KeyValuePair<string, string>("FoodtruckerDatabase", args[0]) }
                    }
                )
                .Build();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            RegisterPersistency(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<AbstractPersistencyContext>();
                var database = context.Database;

                await database.MigrateAsync();
                using (var transaction = await database.BeginTransactionAsync())
                {
                    Persistency.Persistency.OnStart(scope.ServiceProvider);
                    var userManager = scope.ServiceProvider.GetService<UserManager<FoodtruckerUser>>();
                    var email = "contact@miroslawgalczynski.com";
                    FoodtruckerUser user;
                    if (await context.Users.AnyAsync(u => u.Email == email))
                        user = await context.Users.SingleAsync(u => u.Email == email);
                    else
                    {
                        user = new FoodtruckerUser
                        {
                            UserName = email,
                            Email = email,
                            FirstName = "Mirosław",
                            LastName = "Gałczyński"
                        };
                        var result = await userManager.CreateAsync(user, "P@ssw0rd1");
                        if (!result.Succeeded)
                            Exit(result, transaction);
                    }

                    var addToRoleResult = await userManager.AddToRoleAsync(user, FoodtruckerRole.FoodtruckStaff);
                    if (!addToRoleResult.Succeeded)
                        Exit(addToRoleResult, transaction);

                    var foodtruckService = scope.ServiceProvider.GetService<IFoodtruckService>();
                    var foodtruckOwnershipService = scope.ServiceProvider.GetService<IFoodtruckOwnershipService>();

                    // bit ugly
                    await context.Database.ExecuteSqlCommandAsync((string)$@"DELETE FROM ""{nameof(context.FoodtruckOwnerships)}""");
                    await context.Database.ExecuteSqlCommandAsync((string)$@"DELETE FROM ""{nameof(context.Foodtrucks)}""");

                    using (var fileStream = new FileInfo(args[1]).OpenRead())
                    {
                        var source = new PBFOsmStreamSource(fileStream)
                        .Where(e => types.Contains(e.Type) && e.Tags.Any((Tag t) => t.Key == "amenity" && amenities.Contains(t.Value)));
                        var foodtrucks = new InterpretedFeatureStreamSource(
                             new OsmSimpleCompleteStreamSource(source.ShowProgress()),
                            new DefaultFeatureInterpreter()
                        )
                        .AsParallel()
                        .Where(f => amenities.Contains(f.Attributes.GetAtributeOrNull("amenity")))
                        .Select(feature =>
                        {
                            var name = feature.Attributes.GetAtributeOrNull("name")?.ToString() ?? "Without name";
                            return new Foodtruck
                            {
                                DefaultLocation = new NetTopologySuite.Geometries.Point(feature.Geometry.Coordinate) { SRID = feature.Geometry.SRID },
                                DisplayName = name,
                                Name = name
                            };
                        });
                        var results = context.Foodtrucks.AddRangeAsync(foodtrucks);
                        await context.SaveChangesAsync();
                        await context.FoodtruckOwnerships.AddRangeAsync(context.Foodtrucks.Select(f => new FoodtruckOwnership
                        {
                            FoodtruckId = f.Id,
                            Type = OwnershipType.ADMIN,
                            User = user
                        }));
                    }
                    await context.SaveChangesAsync();
                    transaction.Commit();
                }
            }
        }

        public static object GetAtributeOrNull(this IAttributesTable attributesTable, string attributeName)
        {
            return attributesTable.GetNames().Contains(attributeName) ? attributesTable[attributeName] : null;
        }
    }
}