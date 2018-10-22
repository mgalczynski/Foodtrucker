﻿using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NetTopologySuite.Geometries;
using Persistency.Dtos;
using Persistency.Services.Implementations;
using Xunit;

namespace Persistency.Test.Services.Implementations
{
    public class PresenceServiceTests : BaseTests
    {
        private static readonly Func<double, double, Point> CreatePoint = ExtensionMethods.CreatePointWithSrid;

        private readonly Mock<AbstractPersistencyContext> _context = new Mock<AbstractPersistencyContext>();

        private readonly PresenceService _presenceService;

        public PresenceServiceTests()
        {
            Context.Foodtrucks.Add(new Entities.Foodtruck
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
            });
            Context.SaveChanges();
            _context.Setup(context => context.Presences).Returns(Context.Presences);
            _presenceService = new PresenceService(_context.Object);
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