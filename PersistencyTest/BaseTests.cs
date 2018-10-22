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
        protected AbstractPersistencyContext Context { get; } = new TestDbContext();

        protected BaseTests()
        {
            Mapper.Initialize(Persistency.InitializeMapper);
        }

        public virtual void Dispose()
        {
        }
    }
}