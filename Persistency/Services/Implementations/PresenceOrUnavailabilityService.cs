using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Persistency.Dtos;
using Entity = Persistency.Entities.PresenceOrUnavailability;
using Foodtruck = Persistency.Entities.Foodtruck;

namespace Persistency.Services.Implementations
{
    internal class PresenceOrUnavailabilityService : BaseService<Entity, PresenceOrUnavailability>, IPresenceOrUnavailabilityService
    {
        private readonly IInternalFoodtruckService _foodtruckService;

        public PresenceOrUnavailabilityService(IInternalPersistencyContext persistencyContext, IInternalFoodtruckService foodtruckService) : base(persistencyContext)
        {
            _foodtruckService = foodtruckService;
        }

        protected override DbSet<Entity> DbSet => PersistencyContext.PresencesOrUnavailabilities;
        private IQueryable<Entity> Queryable => DbSet.Include(p => p.Foodtruck.Slug);

        public async Task<IList<PresenceOrUnavailability>> FindPresencesOrUnavailabilitiesWithin(Coordinate coordinate, double distance)
        {
            return await PersistencyContext.PresencesOrUnavailabilities.FromSql(
                    $@"SELECT p.*
                       FROM ""{nameof(PersistencyContext.Foodtrucks)}"" f
                           INNER JOIN
                           (
                               SELECT *
                               FROM ""{nameof(PersistencyContext.PresencesOrUnavailabilities)}""
                               WHERE ST_DWithin(""{nameof(Entity.Location)}"", ST_SetSRID(ST_MakePoint(@p0, @p1), 4326), @p2)
                           ) AS p ON f.""{nameof(Foodtruck.Id)}"" = p.""{nameof(Entity.FoodtruckId)}""
                       WHERE NOT f.""{nameof(Foodtruck.Deleted)}""
                    "
                    , coordinate.Longitude, coordinate.Latitude, distance
                )
                .ProjectToListAsync<PresenceOrUnavailability>();
        }

        public async Task<IList<PresenceOrUnavailability>> FindPresencesOrUnavailabilitiesWithin(Coordinate topLeft, Coordinate bottomRight)
        {
            return await PersistencyContext.PresencesOrUnavailabilities.FromSql(
                    $@"SELECT p.*
                       FROM ""{nameof(PersistencyContext.Foodtrucks)}"" f
                           INNER JOIN
                           (
                               SELECT *
                               FROM ""{nameof(PersistencyContext.PresencesOrUnavailabilities)}""
                               WHERE ""{nameof(Entity.Location)}"" && ST_MakeEnvelope(@p0, @p1, @p2, @p3, 4326)
                           ) AS p ON f.""{nameof(Entity.Foodtruck.Id)}"" = p.""{nameof(Entity.FoodtruckId)}""
                       WHERE NOT f.""{nameof(Foodtruck.Deleted)}""
                    "
                    , topLeft.Longitude, topLeft.Latitude, bottomRight.Longitude, bottomRight.Latitude
                )
                .ProjectToListAsync<PresenceOrUnavailability>();
        }

        public async Task<IDictionary<string, IList<PresenceOrUnavailability>>> FindPresencesOrUnavailabilities(ICollection<string> foodtruckSlugs)
        {
            return (await Queryable.Where(e => foodtruckSlugs.Contains(e.Foodtruck.Slug))
                    .OrderBy(p => p.FoodtruckId).ThenBy(p => p.StartTime)
                    .Include(p => p.Foodtruck.Slug)
                    .ToListAsync())
                .Aggregate(new SortedDictionary<string, IList<PresenceOrUnavailability>>(), (acc, pres) =>
                {
                    IList<PresenceOrUnavailability> list;
                    Debug.Assert(pres.Id != null, "pres.Id != null");
                    if (acc.TryGetValue(pres.Foodtruck.Slug, out list))
                    {
                        list = new List<PresenceOrUnavailability>();
                        acc[pres.Foodtruck.Slug] = list;
                    }

                    Debug.Assert(list != null, nameof(list) + " != null");
                    list.Add(Mapper.Map<PresenceOrUnavailability>(pres));
                    return acc;
                });
        }

        public async Task<IList<PresenceOrUnavailability>> FindPresencesOrUnavailabilities(string slug)
        {
            return await Queryable.Where(e => slug == e.Foodtruck.Slug)
                .OrderBy(p => p.StartTime)
                .ProjectToListAsync<PresenceOrUnavailability>();
        }

        private async Task ValidatePresenceOrUnavailability(Guid foodtruckId, CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability, Guid? presenceId = null)
        {
            var startTime = createModifyPresenceOrUnavailability.StartTime;
            var endTime = createModifyPresenceOrUnavailability.EndTime ?? DateTime.MaxValue.AddDays(-1);
            var collidingPresenceOrUnavailability = await DbSet
                .FirstOrDefaultAsync(p =>
                    p.FoodtruckId == foodtruckId &&
                    p.Id != presenceId &&
                    (
                        p.StartTime <= startTime &&
                        p.EndTime >= endTime
                        ||
                        p.StartTime >= startTime &&
                        p.StartTime <= endTime
                        ||
                        p.StartTime >= startTime &&
                        p.EndTime <= endTime
                        ||
                        p.EndTime <= startTime &&
                        p.EndTime >= endTime
                        ||
                        p.StartTime <= startTime &&
                        p.EndTime == null
                    )
                );
            if (collidingPresenceOrUnavailability != null)
                throw new ValidationException<PresenceOrUnavailability>(Mapper.Map<PresenceOrUnavailability>(collidingPresenceOrUnavailability));
        }

        public async Task<PresenceOrUnavailability> CreatePresenceOrUnavailability(string foodtruckSlug, CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability)
        {
            var entity = Mapper.Map<Entity>(createModifyPresenceOrUnavailability);
            var foodtruckId = await _foodtruckService.FindFoodtruckIdBySlug(foodtruckSlug);
            entity.FoodtruckId = foodtruckId;
            await ValidatePresenceOrUnavailability(foodtruckId, createModifyPresenceOrUnavailability);
            return Mapper.Map<PresenceOrUnavailability>(await CreateNewEntity(entity));
        }

        public async Task<PresenceOrUnavailability> ModifyPresenceOrUnavailability(Guid presenceId, CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability)
        {
            var entity = await DbSet.FirstOrDefaultAsync(p => p.Id == presenceId);
            if (entity == null)
                return null;
            Mapper.Map(createModifyPresenceOrUnavailability, entity);
            await ValidatePresenceOrUnavailability(entity.FoodtruckId, createModifyPresenceOrUnavailability);
            await PersistencyContext.SaveChangesAsync();
            return Mapper.Map<PresenceOrUnavailability>(entity);
        }
    }
}