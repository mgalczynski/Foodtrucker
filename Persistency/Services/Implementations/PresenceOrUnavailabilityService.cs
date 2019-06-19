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

        public PresenceOrUnavailabilityService(IInternalPersistencyContext persistencyContext, IRuntimeMapper runtimeMapper, IInternalFoodtruckService foodtruckService) : base(persistencyContext, runtimeMapper)
        {
            _foodtruckService = foodtruckService;
        }

        protected override DbSet<Entity> DbSet => PersistencyContext.PresencesOrUnavailabilities;

        private IQueryable<Entity> Queryable => DbSet
            .Where(p => !p.Foodtruck.Deleted)
            .Include(p => p.Foodtruck);

        private async Task<Guid> FindFoodtruckId(string foodtruckSlug) =>
            await _foodtruckService.FindFoodtruckIdBySlug(foodtruckSlug) ?? throw new ArgumentException("Foodtruck not found");

        public async Task<IList<PresenceOrUnavailability>> FindPresencesOrUnavailabilitiesWithin(Coordinate coordinate, double distance, DateTime startEndTime)
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
                       WHERE NOT f.""{nameof(Foodtruck.Deleted)}"" AND (p.""{nameof(Entity.EndTime)}"" IS NULL OR p.""{nameof(Entity.EndTime)}"" > @p3)
                    "
                    , coordinate.Longitude, coordinate.Latitude, distance, startEndTime
                )
                .ProjectToListAsync<PresenceOrUnavailability>(ConfigurationProvider);
        }

        public async Task<IList<PresenceOrUnavailability>> FindPresencesOrUnavailabilitiesWithin(Coordinate topLeft, Coordinate bottomRight, DateTime startEndTime)
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
                       WHERE NOT f.""{nameof(Foodtruck.Deleted)}"" AND (p.""{nameof(Entity.EndTime)}"" IS NULL OR p.""{nameof(Entity.EndTime)}"" > @p4)
                    "
                    , topLeft.Longitude, topLeft.Latitude, bottomRight.Longitude, bottomRight.Latitude, startEndTime
                )
                .ProjectToListAsync<PresenceOrUnavailability>(ConfigurationProvider);
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
                    Debug.Assert(pres.Id != null, $"{nameof(pres)}.{nameof(pres.Id)} != null");
                    if (acc.TryGetValue(pres.Foodtruck.Slug, out list))
                    {
                        list = new List<PresenceOrUnavailability>();
                        acc[pres.Foodtruck.Slug] = list;
                    }

                    Debug.Assert(list != null, $"{nameof(list)} != null");
                    list.Add(RuntimeMapper.Map<PresenceOrUnavailability>(pres));
                    return acc;
                });
        }

        public async Task<IList<PresenceOrUnavailability>> FindPresencesOrUnavailabilities(string slug)
        {
            return await Queryable.Where(e => slug == e.Foodtruck.Slug)
                .OrderBy(p => p.StartTime)
                .ProjectToListAsync<PresenceOrUnavailability>(ConfigurationProvider);
        }

        public async Task<ResponseWithStatus<PresenceOrUnavailability>> ValidatePresenceOrUnavailability(string foodtruckSlug, CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability)
        {
            try
            {
                await PrivateValidatePresenceOrUnavailability(await FindFoodtruckId(foodtruckSlug), createModifyPresenceOrUnavailability);
            }
            catch (ValidationException<PresenceOrUnavailability> ex)
            {
                return ex.MapToResponse();
            }

            return new ResponseWithStatus<PresenceOrUnavailability>
            {
                Successful = true
            };
        }

        public async Task<ResponseWithStatus<PresenceOrUnavailability>> ValidatePresenceOrUnavailability(Guid presenceId, CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability)
        {
            var entity = await Queryable.FirstOrDefaultAsync(p => p.Id == presenceId);
            if (entity == null)
                return new ResponseWithStatus<PresenceOrUnavailability>
                {
                    Description = "Entity not found",
                    Successful = false
                };
            RuntimeMapper.Map(createModifyPresenceOrUnavailability, entity);

            try
            {
                await PrivateValidatePresenceOrUnavailability(entity.FoodtruckId, createModifyPresenceOrUnavailability, presenceId);
            }
            catch (ValidationException<PresenceOrUnavailability> ex)
            {
                return ex.MapToResponse();
            }

            return new ResponseWithStatus<PresenceOrUnavailability>
            {
                Successful = true
            };
        }

        private async Task PrivateValidatePresenceOrUnavailability(Guid foodtruckId, CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability, Guid? presenceId = null)
        {
            var startTime = createModifyPresenceOrUnavailability.StartTime;
            if (createModifyPresenceOrUnavailability.EndTime != null && createModifyPresenceOrUnavailability.EndTime <= createModifyPresenceOrUnavailability.StartTime)
                throw new ValidationException<PresenceOrUnavailability>("End time cannot be earlier rather than start time");
            var endTime = createModifyPresenceOrUnavailability.EndTime ?? DateTime.MaxValue.AddDays(-1);
            var collidingPresenceOrUnavailability = await Queryable
                .FirstOrDefaultAsync(p =>
                    p.FoodtruckId == foodtruckId &&
                    p.Id != presenceId &&
                    (
                        p.EndTime == null &&
                        endTime > p.StartTime
                        ||
                        p.StartTime <= startTime &&
                        p.EndTime >= endTime
                        ||
                        p.StartTime >= startTime &&
                        p.StartTime < endTime
                        ||
                        p.StartTime >= startTime &&
                        p.EndTime <= endTime
                        ||
                        p.EndTime > startTime &&
                        p.EndTime <= endTime
                    )
                );
            if (collidingPresenceOrUnavailability != null)
                throw new ValidationException<PresenceOrUnavailability>(RuntimeMapper.Map<PresenceOrUnavailability>(collidingPresenceOrUnavailability));
        }

        public async Task<PresenceOrUnavailability> CreatePresenceOrUnavailability(string foodtruckSlug, CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability)
        {
            var entity = RuntimeMapper.Map<Entity>(createModifyPresenceOrUnavailability);
            var foodtruckId = await FindFoodtruckId(foodtruckSlug);
            entity.FoodtruckId = foodtruckId;
            await PrivateValidatePresenceOrUnavailability(foodtruckId, createModifyPresenceOrUnavailability);
            return RuntimeMapper.Map<PresenceOrUnavailability>(await CreateNewEntity(entity));
        }

        public async Task<PresenceOrUnavailability> ModifyPresenceOrUnavailability(Guid presenceId, CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability)
        {
            var entity = await DbSet.FirstOrDefaultAsync(p => p.Id == presenceId);
            if (entity == null)
                return null;
            RuntimeMapper.Map(createModifyPresenceOrUnavailability, entity);
            await PrivateValidatePresenceOrUnavailability(entity.FoodtruckId, createModifyPresenceOrUnavailability, presenceId);
            await PersistencyContext.SaveChangesAsync();
            return RuntimeMapper.Map<PresenceOrUnavailability>(entity);
        }
    }
}