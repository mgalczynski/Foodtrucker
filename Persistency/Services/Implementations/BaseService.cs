using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Persistency.Dtos;
using Persistency.Entities;

namespace Persistency.Services.Implementations
{
    internal abstract class BaseService<TEntity, TDto> : IBaseService<TDto>
        where TEntity : BaseEntity where TDto : class
    {
        protected IInternalPersistencyContext PersistencyContext { get; }
        protected abstract DbSet<TEntity> DbSet { get; }

        protected BaseService(IInternalPersistencyContext persistencyContext)
        {
            PersistencyContext = persistencyContext;
        }

        public async Task<IEnumerable<TDto>> FindById(IEnumerable<Guid> ids) =>
            await DbSet.Where(e => ids.Contains((Guid) e.Id)).ProjectToListAsync<TDto>();

        public async Task<TDto> FindById(Guid id) =>
            await DbSet.Where(e => e.Id == id).SingleOrDefaultAsync().MapAsync<TDto, TEntity>();

        protected async Task<InsertStatus<Guid>> CreateNewEntity<TCreateEntityDto>(TCreateEntityDto createEntity)
        {
            var entityEntry = await DbSet.AddAsync(Mapper.Map<TEntity>(createEntity));
            PersistencyContext.SaveChanges();
            Debug.Assert(entityEntry.Entity.Id != null, "entityEntry.Entity.Id != null");
            return new InsertStatus<Guid> {Id = (Guid) entityEntry.Entity.Id, Successful = true};
        }
    }
}