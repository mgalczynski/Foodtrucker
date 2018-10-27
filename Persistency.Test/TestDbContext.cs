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
            //            optionsBuilder.UseInMemoryDatabase(nameof(Test));
            optionsBuilder.UseNpgsql(
                @"Server=localhost;Port=15432;Database=FoodtruckerTest;Username=postgres;Password=postgres;",
                builder => builder.UseNetTopologySuite());
//            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=FoodtruckerTest;Trusted_Connection=True;", builder => builder.UseNetTopologySuite());
        }
    }
}