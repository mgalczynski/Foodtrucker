using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistency.Entities;

namespace Persistency
{
    public sealed class PersistencyContext : DbContext
    {
        private readonly IConfiguration _config;

        internal DbSet<Foodtruck> Foodtrucks { get; set; }
        internal DbSet<Presence> Presences { get; set; }

        public PersistencyContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetValue<string>("FoodtruckerDatabase"),
                builder => builder.UseNetTopologySuite());
        }
    }
}