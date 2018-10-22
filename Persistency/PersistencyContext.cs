using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistency.Entities;

namespace Persistency
{
    public abstract class AbstractPersistencyContext : IdentityDbContext<IdentityUser>, IInternalPersistencyContext
    {
        public virtual DbSet<Foodtruck> Foodtrucks { get; set; }
        public virtual DbSet<Presence> Presences { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasPostgresExtension("postgis");
        }
    }

    internal sealed class PersistencyContext : AbstractPersistencyContext
    {
        private readonly IConfiguration _config;

        public override DbSet<Foodtruck> Foodtrucks { get; set; }
        public override DbSet<Presence> Presences { get; set; }

        public PersistencyContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(_config.GetValue<string>("FoodtruckerDatabase"),
                builder => builder.UseNetTopologySuite());
        }
    }
}