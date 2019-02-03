using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace Persistency.Test
{
    internal class TestDbContext : AbstractPersistencyContext
    {
        private static readonly LoggerFactory _myLoggerFactory =
            new LoggerFactory(new[]
            {
                new DebugLoggerProvider()
            });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLoggerFactory(_myLoggerFactory);
            var db = Environment.GetEnvironmentVariable("DB") ?? "db";
            var port = Environment.GetEnvironmentVariable("PORT") ?? "5432";
            optionsBuilder.UseNpgsql(
                $@"Server={db};Port={port};Database=FoodtruckerTest;Username=postgres;Password=postgres;",
                builder => builder.UseNetTopologySuite());
        }
    }
}