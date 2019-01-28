using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistency.Dtos;

namespace Persistency.Services
{
    public interface IPresenceService : IBaseService<Presence>
    {
        Task<IList<Presence>> FindPresencesWithin(Coordinate coordinate, double distance);
        Task<IList<Presence>> FindPresences(Guid id);
        Task<IDictionary<Guid, IList<Presence>>> FindPresences(ICollection<string> foodtruckSlug);
        Task<IList<Presence>> FindPresencesWithin(Coordinate topLeft, Coordinate bottomRight);
    }
}