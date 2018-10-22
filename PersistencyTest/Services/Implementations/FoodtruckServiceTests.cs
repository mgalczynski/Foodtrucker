using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using NetTopologySuite.Geometries;
using Persistency.Dtos;
using Persistency.Services.Implementations;
using Xunit;

namespace Persistency.Test.Services.Implementations
{
    public class FoodtruckServiceTests : BaseTests
    {
        private static readonly Func<double, double, Point> CreatePoint = ExtensionMethods.CreatePointWithSrid;

        private readonly Mock<AbstractPersistencyContext> _context = new Mock<AbstractPersistencyContext>();

        private readonly List<Entities.Foodtruck> _list = new List<Entities.Foodtruck>
        {
            new Entities.Foodtruck
            {
                DefaultLocation = CreatePoint(51.125975, 16.978056),
                Name = "Foodtruck within location",
                DisplayName = "Foodtruck within location"
            },
            new Entities.Foodtruck
            {
                DefaultLocation = CreatePoint(51.107261, 17.059999),
                Name = "Foodtruck outside location",
                DisplayName = "Foodtruck outside location"
            },
            new Entities.Foodtruck
            {
                Name = "Foodtruck without location",
                DisplayName = "Foodtruck without location"
            }
        };

        private readonly FoodtruckService _foodtruckService;

        public FoodtruckServiceTests()
        {
            Context.Foodtrucks.AddRange(_list);
            Context.SaveChanges();
            _context.Setup(context => context.Foodtrucks).Returns(Context.Foodtrucks);
            _foodtruckService = new FoodtruckService(_context.Object);
        }

        [Fact]
        public async void Test()
        {
            const double distance = 2d;
            var coordinate = new Coordinate {Latitude = 51.125975, Longitude = 16.978056};
            var result = await _foodtruckService.FindFoodTrucksWithin(coordinate, distance);
            Assert.Equal(new HashSet<string> {"Foodtruck within location"},
                result.Select(foodtruck => foodtruck.Name).ToHashSet());
        }
    }
}