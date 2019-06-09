using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Persistency.Entities;

namespace Persistency.Services.Implementations
{
    internal abstract class BaseService<TEntity, TDto> : IBaseService<TDto>
        where TEntity : BaseEntity, new() where TDto : class
    {
        protected BaseService(IInternalPersistencyContext persistencyContext, IRuntimeMapper runtimeMapper)
        {
            PersistencyContext = persistencyContext;
            RuntimeMapper = runtimeMapper;
        }

        protected IInternalPersistencyContext PersistencyContext { get; }
        protected IRuntimeMapper RuntimeMapper { get; }
        protected IConfigurationProvider ConfigurationProvider => RuntimeMapper.ConfigurationProvider;
        protected abstract DbSet<TEntity> DbSet { get; }

        public async Task<IList<TDto>> FindById(IEnumerable<Guid> ids) =>
            await DbSet.Where(e => ids.Contains(e.Id)).ProjectToListAsync<TDto>(ConfigurationProvider);

        public async Task<TDto> FindById(Guid id) =>
            await DbSet.Where(e => e.Id == id).SingleOrDefaultAsync().MapAsync<TDto, TEntity>(RuntimeMapper);

        public async Task RemoveById(Guid id)
        {
            DbSet.Remove(new TEntity {Id = id});
            await PersistencyContext.SaveChangesAsync();
        }

        protected async Task<TEntity> CreateNewEntity<TCreateEntityDto>(TCreateEntityDto createEntityDto) =>
            await CreateNewEntity(RuntimeMapper.Map<TEntity>(createEntityDto));

        protected async Task<TEntity> CreateNewEntity(TEntity entity)
        {
            var entityEntry = await DbSet.AddAsync(entity);
            await PersistencyContext.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}