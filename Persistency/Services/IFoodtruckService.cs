using System.Collections.Generic;
using System.Threading.Tasks;
using Persistency.Dtos;

namespace Persistency.Services
{
    public interface IFoodtruckService : IBaseService<Foodtruck>
    {
        Task<List<Foodtruck>> FindFoodTrucksWithin(Coordinate coordinate, decimal radius);
    }
}