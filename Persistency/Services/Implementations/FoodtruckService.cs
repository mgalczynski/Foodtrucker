using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NetTopologySuite.Geometries;
using Persistency.Dtos;

namespace Persistency.Services.Implementations
{
    internal class FoodtruckService : IFoodtruckService
    {
        private readonly PersistencyContext _persistencyContext;

        public FoodtruckService(PersistencyContext persistencyContext)
        {
            _persistencyContext = persistencyContext;
        }

        public async Task<List<Foodtruck>> FindFoodTrucksWithin(Coordinate coordinate, decimal radius)
        {
            var dbGeography = Mapper.Map<Point>(coordinate);
            return await (from f in _persistencyContext.Foodtrucks
                where f.DefaultLocation != null && f.DefaultLocation.Distance(dbGeography) < (double) radius
                select f).Take(300).ProjectToListAsync<Foodtruck>();
        }
    }
}
