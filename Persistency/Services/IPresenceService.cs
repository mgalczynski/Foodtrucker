using System.Collections.Generic;
using System.Threading.Tasks;
using Persistency.Dtos;

namespace Persistency.Services
{
    public interface IPresenceService : IBaseService<Presence>
    {
        Task<IList<Presence>> FindPresencesWithin(Coordinate coordinate, double distance);
        Task<IList<Presence>> FindPresences(string slug);
        Task<IDictionary<string, IList<Presence>>> FindPresences(ICollection<string> foodtruckSlug);
        Task<IList<Presence>> FindPresencesWithin(Coordinate topLeft, Coordinate bottomRight);
    }
}