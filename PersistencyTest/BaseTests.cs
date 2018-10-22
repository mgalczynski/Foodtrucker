using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Persistency.Test
{
    public class BaseTests : IDisposable
    {
        protected PersistencyContext Context { get; }

        public BaseTests()
        {
            Mapper.Initialize(Persistency.InitializeMapper);
            var options = new DbContextOptionsBuilder<PersistencyContext>()
                .UseInMemoryDatabase(nameof(Test))
                .Options;
            Context = new PersistencyContext(options);
        }

        public void Dispose()
        {
        }
    }
}