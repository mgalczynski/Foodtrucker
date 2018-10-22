using System;
using System.Collections.Generic;
using System.Linq;
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
                Name = "Food truck within location"
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
            Assert.Contains("Food truck within location", result.Select(foodtruck => foodtruck.Name));
        }
    }
}