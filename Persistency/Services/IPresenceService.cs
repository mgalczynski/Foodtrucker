using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistency.Dtos;

namespace Persistency.Services
{
    internal interface IPresenceService : IBaseService<Presence>
    {
        Task<IList<Presence>> FindPresencesWithin(Coordinate coordinate, double distance);
        Task<IList<Presence>> FindPresences(ICollection<Guid> foodtruckIds);
    }
}