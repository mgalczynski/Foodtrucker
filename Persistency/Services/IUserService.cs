using System.Collections.Generic;
using System.Threading.Tasks;
using Persistency.Dtos;

namespace Persistency.Services
{
    public interface IUserService
    {
        Task<IList<User>> FindByQuery(IEnumerable<string> args, IEnumerable<string> emails);
    }
}