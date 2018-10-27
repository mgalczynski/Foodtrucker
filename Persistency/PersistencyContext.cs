using System;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Persistency.Entities;

namespace Persistency
{
    public abstract class AbstractPersistencyContext : IdentityDbContext<FoodtruckerUser, FoodtruckerRole, Guid>,
        IInternalPersistencyContext
    {
        public DbSet<Foodtruck> Foodtrucks { get; set; }
        public DbSet<Presence> Presences { get; set; }
        public DbSet<FoodtruckOwnership> FoodtruckOwnerships { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<FoodtruckOwnership>().HasKey(e => new {e.UserId, e.FoodtruckId});
            builder.Entity<FoodtruckOwnership>()
                .Property(e => e.Type)
                .HasConversion(new EnumToStringConverter<OwnershipType>())
                .HasMaxLength(Enum.GetValues(typeof(OwnershipType))
                    .Cast<OwnershipType>()
                    .Max(v => v.ToString().Length));
            builder.HasPostgresExtension("postgis");
        }
    }

    internal sealed class PersistencyContext : AbstractPersistencyContext
    {
        private readonly IConfiguration _config;

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