using System.Collections.Generic;
using System.Threading.Tasks;
using Persistency.Dtos;

namespace Persistency.Services
{
    internal interface IPresenceService : IBaseService<Presence>
    {
        Task<List<Presence>> FindFoodTrucksWithin(Coordinate coordinate, decimal radius);
    }
}