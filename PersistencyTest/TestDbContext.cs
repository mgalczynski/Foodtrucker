using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Persistency.Test
{
    internal class TestDbContext : AbstractPersistencyContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //            optionsBuilder.UseInMemoryDatabase(nameof(Test));
            //optionsBuilder.UseSqlServer(@"Server=localhost,11433;Database=Foodtrucker;User=sa;Password=yourStrong(!)Password;", builder => builder.UseNetTopologySuite());
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=FoodtruckerTest;Trusted_Connection=True;", builder => builder.UseNetTopologySuite());
        }
    }
}