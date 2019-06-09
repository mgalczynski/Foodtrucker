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
            services.AddDbContext<PersistencyContext>();
            services.AddScoped<IInternalPersistencyContext, PersistencyContext>(ContextFactory);
            services.AddScoped<IPersistencyContext, PersistencyContext>(ContextFactory);
            services.AddScoped<AbstractPersistencyContext, PersistencyContext>(ContextFactory);
            services.AddScoped<IInternalFoodtruckService, FoodtruckService>();
            services.AddScoped<IFoodtruckService, FoodtruckService>();
            services.AddScoped<IPresenceOrUnavailabilityService, PresenceOrUnavailabilityService>();
            services.AddScoped<IFoodtruckOwnershipService, FoodtruckOwnershipService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISlugHelper, SlugHelper>();
#if DEBUG
            services.AddScoped<DevelopmentPresencesOrUnavailabilitiesFactory>();
#endif
            services.AddIdentity<Entities.FoodtruckerUser, Entities.FoodtruckerRole>()
                .AddEntityFrameworkStores<PersistencyContext>()
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
            services.AddSingleton(CreateMapper());
        }

        private static PersistencyContext ContextFactory(IServiceProvider provider) =>
            provider.GetService<PersistencyContext>();

        internal static IRuntimeMapper CreateMapper() =>
            new Mapper(new MapperConfiguration(InitializeMapper));

        private static void InitializeMapper(IMapperConfigurationExpression mapper)
        {
            mapper.CreateMap<Dtos.CreateModifyFoodtruck, Entities.Foodtruck>();
            mapper.CreateMap<Entities.FoodtruckerUser, Dtos.User>();
            mapper.CreateMap<Entities.Foodtruck, Dtos.Foodtruck>();
            mapper.CreateMap<Entities.PresenceOrUnavailability, Dtos.PresenceOrUnavailability>();
            mapper.CreateMap<Dtos.CreateModifyPresenceOrUnavailability, Entities.PresenceOrUnavailability>();
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
                    .GetService<DevelopmentPresencesOrUnavailabilitiesFactory>()
                    .GenerateDevelopmentPresencesOrUnavailabilities()
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