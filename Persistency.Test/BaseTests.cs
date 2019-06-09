using System;
using AutoMapper;
using Xunit;

namespace Persistency.Test
{
    [Collection("DB Tests")]
    public class BaseTests : IDisposable
    {
        protected BaseTests()
        {
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
        }

        protected AbstractPersistencyContext Context { get; } = new TestDbContext();

        public void Dispose()
        {
        }
    }
}