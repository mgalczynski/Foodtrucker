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
    internal class FoodtruckService : BaseService<Entity, Foodtruck>, IInternalFoodtruckService
    {
        private readonly ISlugHelper _slugHelper;

        public FoodtruckService(IInternalPersistencyContext persistencyContext, IRuntimeMapper runtimeMapper, ISlugHelper slugHelper) : base(persistencyContext, runtimeMapper)
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
                .ProjectToListAsync<Foodtruck>(ConfigurationProvider);

        public async Task<IList<Foodtruck>> FindFoodTrucksWithin(Coordinate topLeft, Coordinate bottomRight) =>
            await PersistencyContext.Foodtrucks.FromSql(
                    $@"SELECT *
                       FROM ""{nameof(PersistencyContext.Foodtrucks)}""
                       WHERE ""{nameof(Entity.DefaultLocation)}"" IS NOT NULL AND NOT ""{nameof(Entity.Deleted)}""
                         AND ""{nameof(Entity.DefaultLocation)}"" && ST_MakeEnvelope(@p0, @p1, @p2, @p3, 4326)"
                    , topLeft.Longitude, topLeft.Latitude, bottomRight.Longitude, bottomRight.Latitude
                )
                .ProjectToListAsync<Foodtruck>(ConfigurationProvider);

        public async Task<FoodtruckDetailed> CreateNewFoodtruck(CreateModifyFoodtruck createNewFoodtruck)
        {
            var entity = RuntimeMapper.Map<Entity>(createNewFoodtruck);
            entity.Slug = await GenerateSlug(createNewFoodtruck.Name);
            return RuntimeMapper.Map<FoodtruckDetailed>(await CreateNewEntity(entity));
        }

        public async Task<FoodtruckDetailed> ModifyFoodtruck(string slug, CreateModifyFoodtruck changeFoodtruck)
        {
            var entity = await DbSet.FirstAsync(f => f.Slug == slug);
            if (entity == null)
                return null;
            RuntimeMapper.Map(changeFoodtruck, entity);
            await PersistencyContext.SaveChangesAsync();
            return RuntimeMapper.Map<FoodtruckDetailed>(entity);
        }

        public async Task MarkAsDeleted(string slug)
        {
            var foodtruck = await DbSet.FirstOrDefaultAsync(e => e.Slug == slug);
            if (foodtruck == null)
                throw new ArgumentException();
            foodtruck.Deleted = true;
            await PersistencyContext.SaveChangesAsync();
        }

        public async Task<Foodtruck> FindBySlug(string slug) =>
            RuntimeMapper.Map<Foodtruck>(await DbSet.FirstOrDefaultAsync(f => f.Slug == slug));

        public async Task<IList<Foodtruck>> FindBySlugs(IEnumerable<string> slugs) =>
            await DbSet.Where(f => slugs.Contains(f.Slug)).ProjectToListAsync<Foodtruck>(ConfigurationProvider);

        public async Task<FoodtruckDetailed> FindBySlugDetailed(string slug) =>
            RuntimeMapper.Map<FoodtruckDetailed>(
                await DbSet
                    .Include(f => f.Ownerships)
                    .ThenInclude(o => o.User)
                    .Include(f => f.PresencesOrUnavailabilities)
                    .FirstOrDefaultAsync(f => f.Slug == slug)
            );

        public async Task<Guid?> FindFoodtruckIdBySlug(string slug) =>
            (await DbSet.FirstOrDefaultAsync(f => f.Slug == slug))?.Id;

        private async Task<string> GenerateSlug(string name, Guid? id = null)
        {
            var slug = _slugHelper.GenerateSlug(name);
            var slugs = (await DbSet
                    .Where(f => f.Slug.StartsWith(slug) && f.Id != id)
                    .Select(f => f.Slug)
                    .ToListAsync())
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
    }
}