using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Persistency.Entities;

namespace Persistency.Services.Implementations
{
    internal abstract class BaseService<TEntity, TDto> : IBaseService<TDto>
        where TEntity : BaseEntity where TDto : class
    {
        protected IInternalPersistencyContext PersistencyContext { get; }
        protected abstract DbSet<TEntity> DbSet { get; }

        public async Task<IEnumerable<TDto>> FindById(IEnumerable<Guid> ids) =>
            await DbSet.Where(e => ids.Contains(e.Id)).ProjectToListAsync<TDto>();

        public async Task<TDto> FindById(Guid id) =>
            await DbSet.Where(e => e.Id == id).SingleOrDefaultAsync().MapAsync<TDto, TEntity>();

        protected BaseService(IInternalPersistencyContext persistencyContext)
        {
            PersistencyContext = persistencyContext;
        }
    }
}