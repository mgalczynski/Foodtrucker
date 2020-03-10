using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Persistency.Dtos;
using Persistency.Services.Implementations;
using Slugify;
using Xunit;
using Foodtruck = Persistency.Entities.Foodtruck;

namespace Persistency.Test.Services.Implementations
{
    public class FoodtruckServiceTests : BaseTests
    {
        public FoodtruckServiceTests()
        {
            var slugHelper = new SlugHelper();
            var foodtrucks = new List<Foodtruck>
            {
                new Foodtruck
                {
                    DefaultLocation = CreatePoint(51.125975, 16.978056),
                    Name = "Foodtruck within location",
                    DisplayName = "Foodtruck within location"
                },
                new Foodtruck
                {
                    DefaultLocation = CreatePoint(51.107261, 17.059999),
                    Name = "Foodtruck outside location",
                    DisplayName = "Foodtruck outside location"
                },
                new Foodtruck
                {
                    Name = "Foodtruck without location",
                    DisplayName = "Foodtruck without location"
                }
            };
            foodtrucks.ForEach(f => f.Slug = slugHelper.GenerateSlug(f.Name));
            Context.Foodtrucks.AddRange(foodtrucks);
            Context.SaveChanges();
            _foodtruckService = new FoodtruckService(Context, Persistency.CreateMapper(), slugHelper);
        }

        private static readonly Func<double, double, Point> CreatePoint = ExtensionMethods.CreatePointWithSrid;

        private readonly FoodtruckService _foodtruckService;

        [Fact]
        public async void CreateFoodTruckTest()
        {
            var result = await _foodtruckService.CreateNewFoodtruck(new CreateModifyFoodtruck
                {Name = "New foodtruck", DisplayName = "New foodtruck"});
            Assert.Equal("New foodtruck",
                Context.Foodtrucks.FirstOrDefault(foodtruck => foodtruck.Slug == result.Slug && foodtruck.Slug == result.Slug)?.Name);
        }

        [Fact]
        public async void CreateFoodTruckTestShouldReturnNotSuccessful()
        {
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _foodtruckService.CreateNewFoodtruck(new CreateModifyFoodtruck {Name = "New foodtruck"}));
            Assert.DoesNotContain("New foodtruck", Context.Foodtrucks.Select(foodtruck => foodtruck.Name));
        }

        [Fact]
        public async void FindFoodtrucksTest()
        {
            double distance = 2;
            var coordinate = new Dtos.Coordinate {Latitude = 51.125975, Longitude = 16.978056};
            var result = await _foodtruckService.FindFoodTrucksWithin(coordinate, distance);
            Assert.Equal(new HashSet<string> {"Foodtruck within location"},
                result.Select(foodtruck => foodtruck.Name).ToHashSet());
            distance = 10000;
            result = await _foodtruckService.FindFoodTrucksWithin(coordinate, distance);
            Assert.Equal(new HashSet<string> {"Foodtruck within location", "Foodtruck outside location"},
                result.Select(foodtruck => foodtruck.Name).ToHashSet());
            var slug = (await Context.Foodtrucks.FirstAsync(e => e.Name == "Foodtruck outside location")).Slug;
            await _foodtruckService.MarkAsDeleted(slug);
            result = await _foodtruckService.FindFoodTrucksWithin(coordinate, distance);
            Assert.Equal(new HashSet<string> {"Foodtruck within location"},
                result.Select(foodtruck => foodtruck.Name).ToHashSet());
        }
    }
}