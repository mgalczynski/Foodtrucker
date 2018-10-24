using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Persistency.Entities;
using Persistency.Services;
using Persistency.Services.Implementations;

namespace Persistency
{
    public static class Persistency
    {
        public static void RegisterPersistency(IServiceCollection services)
        {
            services.AddDbContext<IInternalPersistencyContext, PersistencyContext>();
            services.AddDbContext<IPersistencyContext, PersistencyContext>();
            services.AddDbContext<AbstractPersistencyContext, PersistencyContext>();
            services.AddScoped<IFoodtruckService, FoodtruckService>();
            services.AddScoped<IPresenceService, PresenceService>();
            services.AddIdentity<FoodtruckerUser, FoodtruckerRole>()
                .AddEntityFrameworkStores<AbstractPersistencyContext>().AddDefaultTokenProviders();
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
            mapper.CreateMap<Dtos.Foodtruck, Entities.Foodtruck>().ReverseMap();
            mapper.CreateMap<Dtos.Presence, Entities.Presence>().ReverseMap();
            mapper.CreateMap<Dtos.Coordinate, Point>().ConvertUsing(ExtensionMethods.ToDbPoint);
            mapper.CreateMap<Point, Dtos.Coordinate>().ConvertUsing(ExtensionMethods.ToCoordinate);
        }

        public static void OnStart(RoleManager<FoodtruckerRole> roleManager)
        {
            foreach (var task in FoodtruckerRole.Roles
                .Where(name => !roleManager.RoleExistsAsync(name).Result)
                .Select(name => roleManager.CreateAsync(new FoodtruckerRole {Name = name})))
                if(!task.Result.Succeeded)
                    throw new SystemException(string.Join(", ", task.Result.Errors.Select(error=>error.Description)));
        }
    }
}