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
        public DbSet<PresenceOrUnavailability> PresencesOrUnavailabilities { get; set; }
        public DbSet<FoodtruckOwnership> FoodtruckOwnerships { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Foodtruck>()
                .HasIndex(e => e.Slug)
                .IsUnique()
                .HasName("AK_Foodtrucks_Slug");
            builder.Entity<FoodtruckOwnership>().HasKey(e => new {e.UserId, e.FoodtruckId});
            builder.Entity<FoodtruckOwnership>()
                .Property(e => e.Type)
                .HasConversion(new EnumToStringConverter<OwnershipType>())
                .HasMaxLength(Enum.GetValues(typeof(OwnershipType))
                    .Cast<OwnershipType>()
                    .Max(v => v.ToString().Length));
            builder.Entity<FoodtruckerUser>()
                .HasAlternateKey(e => e.Email)
                .HasName("AK_Users_Mail");
            builder.Entity<FoodtruckerUser>()
                .HasIndex(e => e.NormalizedEmail)
                .IsUnique()
                .HasName("AK_Users_NormalizedEmail");
            builder.HasPostgresExtension("postgis");
        }
    }

    internal sealed class PersistencyContext : AbstractPersistencyContext
    {
        private readonly string _connectionString;

        internal PersistencyContext(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public PersistencyContext(IConfiguration config) : this(config.GetValue<string>("FoodtruckerDatabase"))
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(_connectionString,
                builder => builder.UseNetTopologySuite());
        }
    }
}