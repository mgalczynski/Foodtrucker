using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistency.Dtos;

namespace Persistency.Services
{
    public interface IFoodtruckService : IBaseService<Foodtruck>
    {
        Task<IList<Foodtruck>> FindFoodTrucksWithin(Coordinate coordinate, double distance);
        Task<Guid> CreateNewFoodtruck(CreateNewFoodtruck createNewFoodtruck);
    }
}