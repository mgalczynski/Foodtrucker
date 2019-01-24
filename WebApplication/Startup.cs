using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistency;
using Persistency.Entities;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the React files will be served from this directory
            Persistency.Persistency.RegisterPersistency(services);
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
#if DEBUG
            IHostingEnvironment env,
#endif
            IServiceProvider serviceProvider,
            RoleManager<FoodtruckerRole> roleManager
        )
        {
            var dbContext = serviceProvider.GetService<IPersistencyContext>();

#if DEBUG
            if (env.IsDevelopment())
            {
                dbContext.Database.EnsureCreated();
                app.UseDeveloperExceptionPage();
                app.UseHttpsRedirection();
            }
            else
            {
#endif
                app.UseExceptionHandler("/Error");
                app.UseHsts();
#if DEBUG
            }
#endif
            dbContext.Database.Migrate();

            Persistency.Persistency.OnStart(
#if DEBUG
                env.IsDevelopment(),
#endif
                serviceProvider
                );
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

#if DEBUG
                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
#endif
            });
        }
    }
}