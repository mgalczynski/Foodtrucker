using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Persistency.Dtos;

namespace Persistency.Services.Implementations
{
    internal class FoodtruckService : BaseService<Entities.Foodtruck, Foodtruck>, IFoodtruckService
    {
        protected override DbSet<Entities.Foodtruck> DbSet => PersistencyContext.Foodtrucks;

        public FoodtruckService(IInternalPersistencyContext persistencyContext) : base(persistencyContext)
        {
        }

        public async Task<List<Foodtruck>> FindFoodTrucksWithin(Coordinate coordinate, double radius)
        {
            var dbGeography = Mapper.Map<Point>(coordinate);
            return await (from f in PersistencyContext.Foodtrucks
                where f.DefaultLocation != null && f.DefaultLocation.Distance(dbGeography) < radius
                select f).Take(300).ProjectToListAsync<Foodtruck>();
        }
    }
}