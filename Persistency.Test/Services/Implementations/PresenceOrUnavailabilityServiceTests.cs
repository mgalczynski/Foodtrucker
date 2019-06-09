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
using PresenceOrUnavailability = Persistency.Entities.PresenceOrUnavailability;

namespace Persistency.Test.Services.Implementations
{
    public class PresenceOrUnavailabilityServiceTests : BaseTests
    {
        public PresenceOrUnavailabilityServiceTests()
        {
            var slugHelper = new SlugHelper();
            var foodtrucks = new List<Foodtruck>
            {
                new Foodtruck
                {
                    Name = "Foodtruck without location",
                    DisplayName = "Foodtruck without location",
                    PresencesOrUnavailabilities = new List<PresenceOrUnavailability>
                    {
                        new PresenceOrUnavailability
                        {
                            Location = CreatePoint(51.107261, 17.059999),
                            Title = "PresenceOrUnavailability outside location",
                            StartTime = new DateTime(2019, 1, 3),
                            EndTime = new DateTime(2019, 1, 4)
                        },
                        new PresenceOrUnavailability
                        {
                            Location = CreatePoint(51.125975, 16.978056),
                            Title = "PresenceOrUnavailability within location",
                            StartTime = new DateTime(2019, 1, 5),
                            EndTime = new DateTime(2019, 1, 6)
                        },
                        new PresenceOrUnavailability
                        {
                            Location = CreatePoint(51.107261, 17.059999),
                            Title = "Last presence",
                            StartTime = new DateTime(2019, 1, 7)
                        }
                    }
                },
                new Foodtruck
                {
                    Name = "Foodtruck without location and unavailabilities",
                    DisplayName = "Foodtruck without location and unavailabilities",
                    PresencesOrUnavailabilities = new List<PresenceOrUnavailability>
                    {
                        new PresenceOrUnavailability
                        {
                            Title = "PresenceOrUnavailability outside location",
                            StartTime = new DateTime(2019, 1, 3),
                            EndTime = new DateTime(2019, 1, 4)
                        },
                        new PresenceOrUnavailability
                        {
                            Title = "PresenceOrUnavailability within location",
                            StartTime = new DateTime(2019, 1, 5),
                            EndTime = new DateTime(2019, 1, 6)
                        },
                        new PresenceOrUnavailability
                        {
                            Title = "Last presence",
                            StartTime = new DateTime(2019, 1, 7)
                        }
                    }
                },
                new Foodtruck
                {
                    Name = "Deleted Foodtruck",
                    DisplayName = "Deleted Foodtruck",
                    PresencesOrUnavailabilities = new List<PresenceOrUnavailability>
                    {
                        new PresenceOrUnavailability
                        {
                            Location = CreatePoint(51.125975, 16.978056),
                            Title = "PresenceOrUnavailability within location"
                        }
                    },
                    Deleted = true
                }
            };
            foodtrucks.ForEach(f => f.Slug = slugHelper.GenerateSlug(f.Name));
            Context.Foodtrucks.AddRange(foodtrucks);
            Context.SaveChanges();
            _presenceService = new PresenceOrUnavailabilityService(Context, Persistency.CreateMapper(), new FoodtruckService(Context, Persistency.CreateMapper(), new SlugHelper()));
            _foodtruckWithoutLocation = Context.Foodtrucks.First(f => f.Name == "Foodtruck without location");
            _foodtruckWithoutLocationAndUnavailabilieties = Context.Foodtrucks.First(f => f.Name == "Foodtruck without location and unavailabilities");
        }

        private static readonly Func<double, double, Point> CreatePoint = ExtensionMethods.CreatePointWithSrid;

        private readonly PresenceOrUnavailabilityService _presenceService;
        private readonly Foodtruck _foodtruckWithoutLocation;
        private readonly Foodtruck _foodtruckWithoutLocationAndUnavailabilieties;

        [Fact]
        public async void Test()
        {
            const double distance = 2d;
            var coordinate = new Coordinate {Latitude = 51.125975, Longitude = 16.978056};
            var result = await _presenceService.FindPresencesOrUnavailabilitiesWithin(coordinate, distance);
            Assert.Equal(new HashSet<string> {"PresenceOrUnavailability within location"},
                result.Select(presence => presence.Title).ToHashSet());
        }


        [Fact]
        public async void CannotCreatePresenceOrUnavailabilityForWholeJanuary()
        {
            var title = "Should not be saved";
            await Assert.ThrowsAsync<ValidationException<Dtos.PresenceOrUnavailability>>(() => _presenceService.CreatePresenceOrUnavailability(
                _foodtruckWithoutLocation.Slug,
                new CreateModifyPresenceOrUnavailability
                {
                    Title = title,
                    Description = "PresenceOrUnavailability for whole January",
                    Location = new Coordinate {Latitude = 51.107261, Longitude = 17.059999},
                    StartTime = new DateTime(2019,1,1),
                    EndTime = new DateTime(2019,2,1)
                }
            ));
            await Assert.ThrowsAsync<ValidationException<Dtos.PresenceOrUnavailability>>(() => _presenceService.CreatePresenceOrUnavailability(
                _foodtruckWithoutLocationAndUnavailabilieties.Slug,
                new CreateModifyPresenceOrUnavailability
                {
                    Title = title,
                    Description = "PresenceOrUnavailability for whole January",
                    Location = new Coordinate {Latitude = 51.107261, Longitude = 17.059999},
                    StartTime = new DateTime(2019,1,1),
                    EndTime = new DateTime(2019,2,1)
                }
            ));

            Assert.DoesNotContain(title, await Context.PresencesOrUnavailabilities.Select(p => p.Title).ToListAsync());
        }


        [Fact]
        public async void CannotCreatePresenceOrUnavailabilityStartsFromFifthNoon1()
        {
            var title = "Should not be saved";
            var exception = await Assert.ThrowsAsync<ValidationException<Dtos.PresenceOrUnavailability>>(() => _presenceService.CreatePresenceOrUnavailability(
                _foodtruckWithoutLocation.Slug,
                new CreateModifyPresenceOrUnavailability
                {
                    Title = title,
                    Description = title,
                    Location = new Coordinate {Latitude = 51.107261, Longitude = 17.059999},
                    StartTime = new DateTime(2019,1,5,12,0,0),
                    EndTime = new DateTime(2019,2,1)
                }
            ));

            Assert.Equal("Last presence", exception.Dto.Title);
            Assert.DoesNotContain(title, await Context.PresencesOrUnavailabilities.Select(p => p.Title).ToListAsync());

            exception = await Assert.ThrowsAsync<ValidationException<Dtos.PresenceOrUnavailability>>(() => _presenceService.CreatePresenceOrUnavailability(
                _foodtruckWithoutLocationAndUnavailabilieties.Slug,
                new CreateModifyPresenceOrUnavailability
                {
                    Title = title,
                    Description = title,
                    Location = new Coordinate {Latitude = 51.107261, Longitude = 17.059999},
                    StartTime = new DateTime(2019,1,5,12,0,0),
                    EndTime = new DateTime(2019,2,1)
                }
            ));

            Assert.Equal("Last presence", exception.Dto.Title);
            Assert.DoesNotContain(title, await Context.PresencesOrUnavailabilities.Select(p => p.Title).ToListAsync());
        }


        [Fact]
        public async void CannotCreatePresenceOrUnavailabilityStartsFromFifthNoon2()
        {
            var title = "Should not be saved";
            var exception = await Assert.ThrowsAsync<ValidationException<Dtos.PresenceOrUnavailability>>(() => _presenceService.CreatePresenceOrUnavailability(
                _foodtruckWithoutLocation.Slug,
                new CreateModifyPresenceOrUnavailability
                {
                    Title = title,
                    Description = title,
                    Location = new Coordinate {Latitude = 51.107261, Longitude = 17.059999},
                    StartTime = new DateTime(2019,1,5,12,0,0)
                }
            ));
            
            Assert.Equal("Last presence", exception.Dto.Title);
            Assert.DoesNotContain(title, await Context.PresencesOrUnavailabilities.Select(p => p.Title).ToListAsync());

            exception = await Assert.ThrowsAsync<ValidationException<Dtos.PresenceOrUnavailability>>(() => _presenceService.CreatePresenceOrUnavailability(
                _foodtruckWithoutLocationAndUnavailabilieties.Slug,
                new CreateModifyPresenceOrUnavailability
                {
                    Title = title,
                    Description = title,
                    Location = new Coordinate {Latitude = 51.107261, Longitude = 17.059999},
                    StartTime = new DateTime(2019,1,5,12,0,0)
                }
            ));
            
            Assert.Equal("Last presence", exception.Dto.Title);
            Assert.DoesNotContain(title, await Context.PresencesOrUnavailabilities.Select(p => p.Title).ToListAsync());
        }
    }
}