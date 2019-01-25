using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Persistency.Dtos;
using Persistency.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        public SampleDataController(IFoodtruckService foodtruckService)
        {
            _foodtruckService = foodtruckService;
        }

        private static string[] _summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IFoodtruckService _foodtruckService;

        [HttpGet("[action]")]
        public async Task<IEnumerable<WeatherForecast>> WeatherForecasts(int startDateIndex)
        {
            var foodTruck = await _foodtruckService.FindById(Guid.Parse("{030B4A82-1B7C-11CF-9D53-00AA003C9CB6}"));
            const double distance = 2d;
            var coordinate = new Coordinate {Latitude = 51.125975, Longitude = 16.978056};
            var result = await _foodtruckService.FindFoodTrucksWithin(coordinate, distance);
            Console.WriteLine(await _foodtruckService.FindFoodTrucksWithin(new Coordinate(), 0));
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index + startDateIndex).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = _summaries[rng.Next(_summaries.Length)]
            });
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get { return 32 + (int) (TemperatureC / 0.5556); }
            }
        }
    }
}