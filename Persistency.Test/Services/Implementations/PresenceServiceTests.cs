using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using Persistency.Dtos;
using Persistency.Services.Implementations;
using Slugify;
using Xunit;
using Foodtruck = Persistency.Entities.Foodtruck;

namespace Persistency.Test.Services.Implementations
{
    public class PresenceServiceTests : BaseTests
    {
        private static readonly Func<double, double, Point> CreatePoint = ExtensionMethods.CreatePointWithSrid;

        private readonly PresenceService _presenceService;

        public PresenceServiceTests()
        {
            var slugHelper = new SlugHelper();
            var foodtrucks = new List<Foodtruck>
            {
                new Foodtruck
                {
                    Name = "Foodtruck without location",
                    DisplayName = "Foodtruck without location",
                    Presences = new List<Entities.Presence>
                    {
                        new Entities.Presence
                        {
                            Location = CreatePoint(51.125975, 16.978056),
                            Title = "Presence within location"
                        },
                        new Entities.Presence
                        {
                            Location = CreatePoint(51.107261, 17.059999),
                            Title = "Presence outside location"
                        }
                    }
                },
                new Foodtruck
                {
                    Name = "Deleted Foodtruck",
                    DisplayName = "Deleted Foodtruck",
                    Presences = new List<Entities.Presence>
                    {
                        new Entities.Presence
                        {
                            Location = CreatePoint(51.125975, 16.978056),
                            Title = "Presence within location"
                        }
                    },
                    Deleted = true
                }
            };
            foodtrucks.ForEach(f => f.Slug = slugHelper.GenerateSlug(f.Name));
            Context.Foodtrucks.AddRange(foodtrucks);
            Context.SaveChanges();
            _presenceService = new PresenceService(Context, new FoodtruckService(Context, new SlugHelper()));
        }

        [Fact]
        public async void Test()
        {
            const double distance = 2d;
            var coordinate = new Coordinate {Latitude = 51.125975, Longitude = 16.978056};
            var result = await _presenceService.FindPresencesWithin(coordinate, distance);
            Assert.Equal(new HashSet<string> {"Presence within location"},
                result.Select(presence => presence.Title).ToHashSet());
        }
    }
}