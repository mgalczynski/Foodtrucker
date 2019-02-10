using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Persistency.Dtos;
using Entity = Persistency.Entities.Presence;
using Foodtruck = Persistency.Entities.Foodtruck;

namespace Persistency.Services.Implementations
{
    internal class PresenceService : BaseService<Entity, Presence>, IPresenceService
    {
        private readonly IInternalFoodtruckService _foodtruckService;

        public PresenceService(IInternalPersistencyContext persistencyContext, IInternalFoodtruckService foodtruckService) : base(persistencyContext)
        {
            _foodtruckService = foodtruckService;
        }

        protected override DbSet<Entity> DbSet => PersistencyContext.Presences;
        private IQueryable<Entity> Queryable => DbSet.Include(p => p.Foodtruck.Slug);

        public async Task<IList<Presence>> FindPresencesWithin(Coordinate coordinate, double distance)
        {
            return await PersistencyContext.Presences.FromSql(
                    $@"SELECT p.*
                       FROM ""{nameof(PersistencyContext.Foodtrucks)}"" f
                           INNER JOIN
                           (
                               SELECT *
                               FROM ""{nameof(PersistencyContext.Presences)}""
                               WHERE ST_DWithin(""{nameof(Entity.Location)}"", ST_SetSRID(ST_MakePoint(@p0, @p1), 4326), @p2)
                           ) AS p ON f.""{nameof(Foodtruck.Id)}"" = p.""{nameof(Entity.FoodtruckId)}""
                       WHERE NOT f.""{nameof(Foodtruck.Deleted)}""
"
                    , coordinate.Longitude, coordinate.Latitude, distance
                )
                .ProjectToListAsync<Presence>();
        }

        public async Task<IList<Presence>> FindPresencesWithin(Coordinate topLeft, Coordinate bottomRight)
        {
            return await PersistencyContext.Presences.FromSql(
                    $@"SELECT p.*
                       FROM ""{nameof(PersistencyContext.Foodtrucks)}"" f
                           INNER JOIN
                           (
                               SELECT *
                               FROM ""{nameof(PersistencyContext.Presences)}""
                               WHERE ""{nameof(Entity.Location)}"" && ST_MakeEnvelope(@p0, @p1, @p2, @p3, 4326)
                           ) AS p ON f.""{nameof(Entity.Foodtruck.Id)}"" = p.""{nameof(Entity.FoodtruckId)}""
                       WHERE NOT f.""{nameof(Foodtruck.Deleted)}""
"
                    , topLeft.Longitude, topLeft.Latitude, bottomRight.Longitude, bottomRight.Latitude
                )
                .ProjectToListAsync<Presence>();
        }

        public async Task<IDictionary<string, IList<Presence>>> FindPresences(ICollection<string> foodtruckSlugs)
        {
            return (await Queryable.Where(e => foodtruckSlugs.Contains(e.Foodtruck.Slug))
                    .OrderBy(p => p.FoodtruckId).ThenBy(p => p.StartTime)
                    .Include(p => p.Foodtruck.Slug)
                    .ToListAsync())
                .Aggregate(new SortedDictionary<string, IList<Presence>>(), (acc, pres) =>
                {
                    IList<Presence> list;
                    Debug.Assert(pres.Id != null, "pres.Id != null");
                    if (acc.TryGetValue(pres.Foodtruck.Slug, out list))
                    {
                        list = new List<Presence>();
                        acc[pres.Foodtruck.Slug] = list;
                    }

                    Debug.Assert(list != null, nameof(list) + " != null");
                    list.Add(Mapper.Map<Presence>(pres));
                    return acc;
                });
        }

        public async Task<IList<Presence>> FindPresences(string slug)
        {
            return await Queryable.Where(e => slug == e.Foodtruck.Slug)
                .OrderBy(p => p.StartTime)
                .ProjectToListAsync<Presence>();
        }

        public async Task<Presence> CreatePresence(string foodtruckSlug, CreateModifyPresence createModifyPresence)
        {
            var entity = Mapper.Map<Entity>(createModifyPresence);
            entity.FoodtruckId = await _foodtruckService.FindFoodtruckIdBySlug(foodtruckSlug);
            return Mapper.Map<Presence>(await CreateNewEntity(entity));
        }

        public async Task<Presence> ModifyPresence(Guid presenceId, CreateModifyPresence createModifyPresence)
        {
            var entity = await DbSet.FirstOrDefaultAsync(p => p.Id == presenceId);
            if (entity == null)
                return null;
            Mapper.Map(createModifyPresence, entity);
            await PersistencyContext.SaveChangesAsync();
            return Mapper.Map<Presence>(entity);
        }
    }
}