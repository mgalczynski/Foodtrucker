using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Persistency.Dtos;
using Slugify;
using Entity = Persistency.Entities.Foodtruck;

namespace Persistency.Services.Implementations
{
    internal class FoodtruckService : BaseService<Entity, Foodtruck>, IFoodtruckService
    {
        private readonly ISlugHelper _slugHelper;

        public FoodtruckService(IInternalPersistencyContext persistencyContext, ISlugHelper slugHelper) : base(persistencyContext)
        {
            _slugHelper = slugHelper;
        }

        protected override DbSet<Entity> DbSet => PersistencyContext.Foodtrucks;


        public async Task<IList<Foodtruck>> FindFoodTrucksWithin(Coordinate coordinate, double distance) =>
            await PersistencyContext.Foodtrucks.FromSql(
                    $@"SELECT *
                       FROM ""{nameof(PersistencyContext.Foodtrucks)}""
                       WHERE ""{nameof(Entity.DefaultLocation)}"" IS NOT NULL AND NOT ""{nameof(Entity.Deleted)}""
                         AND ST_DWithin(""{nameof(Entity.DefaultLocation)}"", ST_SetSRID(ST_MakePoint(@p0, @p1), 4326), @p2)"
                    , coordinate.Longitude, coordinate.Latitude, distance
                )
                .ProjectToListAsync<Foodtruck>();

        public async Task<IList<Foodtruck>> FindFoodTrucksWithin(Coordinate topLeft, Coordinate bottomRight) =>
            await PersistencyContext.Foodtrucks.FromSql(
                    $@"SELECT *
                       FROM ""{nameof(PersistencyContext.Foodtrucks)}""
                       WHERE ""{nameof(Entity.DefaultLocation)}"" IS NOT NULL AND NOT ""{nameof(Entity.Deleted)}""
                         AND ""{nameof(Entity.DefaultLocation)}"" && ST_MakeEnvelope(@p0, @p1, @p2, @p3, 4326)"
                    , topLeft.Longitude, topLeft.Latitude, bottomRight.Longitude, bottomRight.Latitude
                )
                .ProjectToListAsync<Foodtruck>();

        public async Task<Foodtruck> CreateNewFoodtruck(CreateNewFoodtruck createNewFoodtruck)
        {
            var entity = Mapper.Map<Entity>(createNewFoodtruck);
            entity.Slug = await GenerateSlug(createNewFoodtruck.Name);
            return Mapper.Map<Foodtruck>(await CreateNewEntity(entity));
        }

        public async Task MarkAsDeleted(Guid id)
        {
            var foodtruck = await DbSet.FirstOrDefaultAsync(e => e.Id == id);
            if (foodtruck == null)
                throw new ArgumentException();
            foodtruck.Deleted = true;
            await PersistencyContext.SaveChangesAsync();
        }

        private async Task<string> GenerateSlug(string name)
        {
            var slug = _slugHelper.GenerateSlug(name);
            var slugs = (await DbSet.ToAsyncEnumerable()
                    .Where(f => f.Slug.StartsWith(slug))
                    .Select(f => f.Slug)
                    .ToList())
                .ToHashSet();
            if (!slugs.Contains(slug))
                return slug;
            for (ulong i = 1;; ++i)
            {
                var slugWithNumber = slug + i;
                if (!slugs.Contains(slugWithNumber))
                    return slugWithNumber;
                if (ulong.MaxValue == i)
                    throw new SystemException("Illegal state");
            }
        }

        public async Task<Foodtruck> FindBySlug(string slug) =>
            Mapper.Map<Foodtruck>(await DbSet.FirstOrDefaultAsync(f => f.Slug == slug));

        public async Task<IList<Foodtruck>> FindBySlugs(IEnumerable<string> slugs) =>
            await DbSet.Where(f => slugs.Contains(f.Slug)).ProjectToListAsync<Foodtruck>();

        public async Task<FoodtruckDetailed> FindBySlugDetailed(string slug) =>
            Mapper.Map<FoodtruckDetailed>(
                await DbSet
                    .Include(f => f.Ownerships)
                    .ThenInclude(o => o.User)
                    .Include(f => f.Presences)
                    .FirstOrDefaultAsync(f => f.Slug == slug)
            );
    }
}