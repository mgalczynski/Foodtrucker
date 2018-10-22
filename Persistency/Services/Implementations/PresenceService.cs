using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Persistency.Dtos;

namespace Persistency.Services.Implementations
{
    internal class PresenceService : BaseService<Entities.Presence, Presence>, IPresenceService
    {
        protected override DbSet<Entities.Presence> DbSet => PersistencyContext.Presences;

        public PresenceService(PersistencyContext persistencyContext) : base(persistencyContext)
        {
        }

        public async Task<List<Presence>> FindFoodTrucksWithin(Coordinate coordinate, decimal radius)
        {
            var dbGeography = Mapper.Map<Point>(coordinate);
            return await (from f in PersistencyContext.Presences
                where f.Location.Distance(dbGeography) < (double) radius
                select f).Take(300).ProjectToListAsync<Presence>();
        }
    }
}