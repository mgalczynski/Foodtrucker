using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Persistency.Dtos;
using Persistency.Services;
using Persistency.Services.Implementations;
using Slugify;
using Xunit;
using Foodtruck = Persistency.Entities.Foodtruck;
using Presence = Persistency.Entities.Presence;

namespace Persistency.Test.Services.Implementations
{
    public class PresenceServiceTests : BaseTests
    {
        public PresenceServiceTests()
        {
            var slugHelper = new SlugHelper();
            var foodtrucks = new List<Foodtruck>
            {
                new Foodtruck
                {
                    Name = "Foodtruck without location",
                    DisplayName = "Foodtruck without location",
                    Presences = new List<Presence>
                    {
                        new Presence
                        {
                            Location = CreatePoint(51.107261, 17.059999),
                            Title = "Presence outside location",
                            StartTime = new DateTime(2019, 1, 3),
                            EndTime = new DateTime(2019, 1, 4)
                        },
                        new Presence
                        {
                            Location = CreatePoint(51.125975, 16.978056),
                            Title = "Presence within location",
                            StartTime = new DateTime(2019, 1, 5),
                            EndTime = new DateTime(2019, 1, 6)
                        },
                        new Presence
                        {
                            Location = CreatePoint(51.107261, 17.059999),
                            Title = "Last presence",
                            StartTime = new DateTime(2019, 1, 7)
                        }
                    }
                },
                new Foodtruck
                {
                    Name = "Deleted Foodtruck",
                    DisplayName = "Deleted Foodtruck",
                    Presences = new List<Presence>
                    {
                        new Presence
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
            _foodtruckWithoutLocation = Context.Foodtrucks.First(f => f.Name == "Foodtruck without location");
        }

        private static readonly Func<double, double, Point> CreatePoint = ExtensionMethods.CreatePointWithSrid;

        private readonly PresenceService _presenceService;
        private readonly Foodtruck _foodtruckWithoutLocation;

        [Fact]
        public async void Test()
        {
            const double distance = 2d;
            var coordinate = new Coordinate {Latitude = 51.125975, Longitude = 16.978056};
            var result = await _presenceService.FindPresencesWithin(coordinate, distance);
            Assert.Equal(new HashSet<string> {"Presence within location"},
                result.Select(presence => presence.Title).ToHashSet());
        }


        [Fact]
        public async void CannotCreatePresenceForWholeJanuary()
        {
            var title = "Should not be saved";
            await Assert.ThrowsAsync<ValidationException<Dtos.Presence>>(() => _presenceService.CreatePresence(
                _foodtruckWithoutLocation.Slug,
                new CreateModifyPresence
                {
                    Title = title,
                    Description = "Presence for whole January",
                    Location = new Coordinate {Latitude = 51.107261, Longitude = 17.059999},
                    StartTime = new DateTime(2019,1,1),
                    EndTime = new DateTime(2019,2,1)
                }
            ));

            Assert.DoesNotContain(title, await Context.Presences.Select(p => p.Title).ToListAsync());
        }


        [Fact]
        public async void CannotCreatePresenceStartsFromFifthNoon1()
        {
            var title = "Should not be saved";
            var exception = await Assert.ThrowsAsync<ValidationException<Dtos.Presence>>(() => _presenceService.CreatePresence(
                _foodtruckWithoutLocation.Slug,
                new CreateModifyPresence
                {
                    Title = title,
                    Description = title,
                    Location = new Coordinate {Latitude = 51.107261, Longitude = 17.059999},
                    StartTime = new DateTime(2019,1,5,12,0,0),
                    EndTime = new DateTime(2019,2,1)
                }
            ));

            Assert.Equal("Last presence", exception.Dto.Title);
            Assert.DoesNotContain(title, await Context.Presences.Select(p => p.Title).ToListAsync());
        }


        [Fact]
        public async void CannotCreatePresenceStartsFromFifthNoon2()
        {
            var title = "Should not be saved";
            var exception = await Assert.ThrowsAsync<ValidationException<Dtos.Presence>>(() => _presenceService.CreatePresence(
                _foodtruckWithoutLocation.Slug,
                new CreateModifyPresence
                {
                    Title = title,
                    Description = title,
                    Location = new Coordinate {Latitude = 51.107261, Longitude = 17.059999},
                    StartTime = new DateTime(2019,1,5,12,0,0)
                }
            ));
            
            Assert.Equal("Last presence", exception.Dto.Title);
            Assert.DoesNotContain(title, await Context.Presences.Select(p => p.Title).ToListAsync());
        }
    }
}