using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Persistency.Services;
using Persistency.Services.Implementations;
using Slugify;

namespace Persistency
{
    public static class Persistency
    {
        public static void RegisterPersistency(IServiceCollection services)
        {
            services.AddDbContext<IInternalPersistencyContext, PersistencyContext>();
            services.AddDbContext<IPersistencyContext, PersistencyContext>();
            services.AddDbContext<AbstractPersistencyContext, PersistencyContext>();
            services.AddScoped<IInternalFoodtruckService, FoodtruckService>();
            services.AddScoped<IFoodtruckService, FoodtruckService>();
            services.AddScoped<IPresenceService, PresenceService>();
            services.AddScoped<IFoodtruckOwnershipService, FoodtruckOwnershipService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISlugHelper, SlugHelper>();
#if DEBUG
            services.AddScoped<DevelopmentPresencesFactory>();
#endif
            services.AddIdentity<Entities.FoodtruckerUser, Entities.FoodtruckerRole>()
                .AddEntityFrameworkStores<AbstractPersistencyContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_@.";
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            Mapper.Initialize(InitializeMapper);
        }

        internal static void InitializeMapper(IMapperConfigurationExpression mapper)
        {
            Func<Dtos.Coordinate, Point> func = ExtensionMethods.ToDbPoint;
            mapper.CreateMap<Dtos.CreateModifyFoodtruck, Entities.Foodtruck>();
            mapper.CreateMap<Dtos.Foodtruck, Entities.Foodtruck>().ReverseMap();
            mapper.CreateMap<Dtos.Presence, Entities.Presence>().ReverseMap();
            mapper.CreateMap<Dtos.Coordinate, Point>().ConvertUsing(c => c.ToDbPoint());
            mapper.CreateMap<Point, Dtos.Coordinate>().ConvertUsing(p => p.ToCoordinate());
            mapper.CreateMap<Enum, string>().IncludeAllDerived().ConvertUsing(e => e.ToString());
        }

        public static void OnStart(
#if DEBUG
            bool isDevelopment,
#endif
            IServiceProvider serviceProvider
        )
        {
            CreateRoles(serviceProvider.GetService<RoleManager<Entities.FoodtruckerRole>>());
#if DEBUG
            if (isDevelopment)
                serviceProvider
                    .GetService<DevelopmentPresencesFactory>()
                    .GenerateDevelopmentPresences()
                    .Wait();
#endif
        }

        internal static void CreateRoles(RoleManager<Entities.FoodtruckerRole> roleManager)
        {
            var rolesInDbNormalizedNames = roleManager.Roles.Select(r => r.NormalizedName).ToHashSet();
            foreach (var r in Entities.FoodtruckerRole.Roles
                .Where(r => !rolesInDbNormalizedNames.Contains(roleManager.NormalizeKey(r))))
                roleManager.CreateAsync(new Entities.FoodtruckerRole {Name = r}).Wait();
        }
}
}