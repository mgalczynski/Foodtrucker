using System;
using AutoMapper;
using Xunit;

namespace Persistency.Test
{
    [Collection("DB Tests")]
    public class BaseTests : IDisposable
    {
        protected AbstractPersistencyContext Context { get; } = new TestDbContext();
        private static object monitor = new object();
        private static bool _mapperWasInitialized = false;

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

        public void Dispose()
        {
//            Context.Database.EnsureDeleted();
        }
    }
}