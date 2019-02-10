using System;
using AutoMapper;
using Xunit;

namespace Persistency.Test
{
    [Collection("DB Tests")]
    public class BaseTests : IDisposable
    {
        private static readonly object monitor = new object();
        private static bool _mapperWasInitialized;

        protected BaseTests()
        {
            lock (monitor)
            {
                if (!_mapperWasInitialized)
                {
                    Mapper.Initialize(Persistency.InitializeMapper);
                    _mapperWasInitialized = true;
                }
            }

            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
        }

        protected AbstractPersistencyContext Context { get; } = new TestDbContext();

        public void Dispose()
        {
//            Context.Database.EnsureDeleted();
        }
    }
}