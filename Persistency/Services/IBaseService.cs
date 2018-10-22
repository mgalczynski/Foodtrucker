using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistency.Services
{
    public interface IBaseService<TDto> where TDto : class
    {
        Task<IEnumerable<TDto>> FindById(IEnumerable<Guid> ids);
        Task<TDto> FindById(Guid id);
    }
}