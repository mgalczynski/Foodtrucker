using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Persistency.Dtos;
using Entity = Persistency.Entities.Presence;

namespace Persistency.Services.Implementations
{
    internal class PresenceService : BaseService<Entity, Presence>, IPresenceService
    {
        protected override DbSet<Entity> DbSet => PersistencyContext.Presences;

        public PresenceService(IInternalPersistencyContext persistencyContext) : base(persistencyContext)
        {
        }

        public async Task<IList<Presence>> FindPresencesWithin(Coordinate coordinate, double distance) =>
            await PersistencyContext.Presences.FromSql(
                    $@"SELECT p.*
                       FROM ""{nameof(PersistencyContext.Foodtrucks)}"" f
                           INNER JOIN
                           (
                               SELECT *
                               FROM ""{nameof(PersistencyContext.Presences)}""
                               WHERE ST_DWithin(""{nameof(Entity.Location)}"", ST_SetSRID(ST_MakePoint(@p0, @p1), 4326), @p2)
                           ) AS p ON f.""{nameof(Entities.Foodtruck.Id)}"" = p.""{nameof(Entity.FoodtruckId)}""
                       WHERE NOT f.""{nameof(Entities.Foodtruck.Deleted)}""
"
                    , coordinate.Longitude, coordinate.Latitude, distance
                )
                .ProjectToListAsync<Presence>();

        public async Task<IList<Presence>> FindPresencesWithin(Coordinate topLeft, Coordinate bottomRight) =>
            await PersistencyContext.Presences.FromSql(
                    $@"SELECT p.*
                       FROM ""{nameof(PersistencyContext.Foodtrucks)}"" f
                           INNER JOIN
                           (
                               SELECT *
                               FROM ""{nameof(PersistencyContext.Presences)}""
                               WHERE ""{nameof(Entity.Location)}"" && ST_MakeEnvelope(@p0, @p1, @p2, @p3, 4326)
                           ) AS p ON f.""{nameof(Entities.Foodtruck.Id)}"" = p.""{nameof(Presence.FoodtruckId)}""
                       WHERE NOT f.""{nameof(Entities.Foodtruck.Deleted)}""
"
                    , topLeft.Longitude, topLeft.Latitude, bottomRight.Longitude, bottomRight.Latitude
                )
                .ProjectToListAsync<Presence>();

        public async Task<IDictionary<Guid, IList<Presence>>> FindPresences(ICollection<Guid> foodtruckIds) =>
            (await DbSet.Where(e => foodtruckIds.Contains(e.FoodtruckId))
                .OrderBy(p => p.FoodtruckId).ThenBy(p => p.StartTime)
                .ToListAsync())
            .Aggregate(new SortedDictionary<Guid, IList<Presence>>(), (acc, pres) =>
            {
                IList<Presence> list;
                if (acc.TryGetValue(pres.Id.Value, out list))
                {
                    list = new List<Presence>();
                    acc[pres.Id.Value] = list;
                }

                list.Add(Mapper.Map<Presence>(pres));
                return acc;
            });

        public async Task<IList<Presence>> FindPresences(Guid id) =>
            await DbSet.Where(e => id == e.FoodtruckId)
                .OrderBy(p => p.StartTime)
                .ProjectToListAsync<Presence>();
    }
}