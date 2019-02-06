#if DEBUG
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Operation.Distance;
using Persistency.Dtos;
using Persistency.Services;
using Presence = Persistency.Entities.Presence;

namespace Persistency
{
    internal class DevelopmentPresencesFactory
    {
        private readonly AbstractPersistencyContext _context;
        private readonly IFoodtruckService _foodtruckService;
        private readonly IPresenceService _presenceService;
        private readonly Random _random;

        public DevelopmentPresencesFactory(IFoodtruckService foodtruckService, IPresenceService presenceService, AbstractPersistencyContext context)
        {
            _foodtruckService = foodtruckService;
            _presenceService = presenceService;
            _context = context;
            _random = new Random();
        }

        internal async Task GenerateDevelopmentPresences()
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var coord = new Coordinate
                {
                    Latitude = 51.110254,
                    Longitude = 17.031626
                };
                double minLat = 51.10473, minLon = 17.0085913, maxLat = 51.1104457, maxLon = 17.0520486;
                var point = coord.ToDbPoint();
                var dayBeforeYesterday = DateTime.Now.AddDays(-2);
                var yesterday = DateTime.Now.AddDays(-1);
                var tomorrow = DateTime.Now.AddDays(1);
                var dayAfterTomorrow = DateTime.Now.AddDays(2);
                foreach (var foodtruck in (await _foodtruckService.FindFoodTrucksWithin(coord, 2000))
                    .Take(300)
                    .OrderBy(p => DistanceOp.Distance(point, p.DefaultLocation.ToDbPoint())))
                {
                    if ((await _presenceService.FindPresences(foodtruck.Slug)).Any())
                        continue;
                    var foodtruckId = (await _context.Foodtrucks.FirstAsync(f => f.Slug == foodtruck.Slug)).Id;
                    await _context.Presences.AddAsync(new Presence
                    {
                        FoodtruckId = foodtruckId,
                        Description = $"Presence of {foodtruck.DisplayName}",
                        Title = $"Presence of {foodtruck.DisplayName}",
                        Location = new Coordinate
                        {
                            Latitude = minLat + _random.NextDouble() * (maxLat - minLat),
                            Longitude = minLon + _random.NextDouble() * (maxLon - minLon)
                        }.ToDbPoint(),
                        StartTime = new DateTime(dayBeforeYesterday.Ticks + (long) _random.NextDouble() * (yesterday.Ticks - dayBeforeYesterday.Ticks)),
                        EndTime = _random.Next(2) == 1 ? new DateTime(dayBeforeYesterday.Ticks + (long) _random.NextDouble() * (yesterday.Ticks - dayBeforeYesterday.Ticks)) : (DateTime?) null
                    });
                }

                await _context.SaveChangesAsync();
                transaction.Commit();
            }
        }
    }
}
#endif