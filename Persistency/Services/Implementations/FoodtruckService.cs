using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Persistency.Dtos;
using Entity = Persistency.Entities.Foodtruck;

namespace Persistency.Services.Implementations
{
    internal class FoodtruckService : BaseService<Entity, Foodtruck>, IFoodtruckService
    {
        protected override DbSet<Entity> DbSet => PersistencyContext.Foodtrucks;

        public FoodtruckService(IInternalPersistencyContext persistencyContext) : base(persistencyContext)
        {
        }

        public async Task<IList<Foodtruck>> FindFoodTrucksWithin(Coordinate coordinate, double distance) =>
            await PersistencyContext.Foodtrucks.FromSql(
                    $@"SELECT *
                       FROM ""{nameof(PersistencyContext.Foodtrucks)}""
                       WHERE ""{nameof(Entity.DefaultLocation)}"" IS NOT NULL AND NOT ""{nameof(Entity.Removed)}""
                         AND ST_DWithin(""{nameof(Entity.DefaultLocation)}"", ST_SetSRID(ST_MakePoint(@p0, @p1), 4326), @p2)"
                    , coordinate.Longitude, coordinate.Latitude, distance
                )
                .ProjectToListAsync<Foodtruck>();

        public async Task<Guid> CreateNewFoodtruck(CreateNewFoodtruck createNewFoodtruck) =>
            await CreateNewEntity(createNewFoodtruck);

        public async Task MarkAsDeleted(Guid id)
        {
            var foodtruck = await DbSet.FirstOrDefaultAsync(e => e.Id == id);
            if (foodtruck == null)
                throw new ArgumentException();
            foodtruck.Removed = true;
            await PersistencyContext.SaveChangesAsync();
        }
    }
}