using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistency;
using Persistency.Dtos;
using Persistency.Entities;
using Persistency.Services;
using Foodtruck = Persistency.Dtos.Foodtruck;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class FoodtruckController : BaseController
    {
        private readonly IFoodtruckService _foodtruckService;
        private readonly IPresenceService _presenceService;

        public FoodtruckController(IFoodtruckService foodtruckService,
            IPresenceService presenceService,
            UserManager<FoodtruckerUser> userManager,
            IPersistencyContext persistencyContext) : base(userManager, persistencyContext)
        {
            _foodtruckService = foodtruckService;
            _presenceService = presenceService;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<GenericListResult<Foodtruck>>> Find([FromBody] FoodtruckQuery foodtruckQuery)
        {
            if (foodtruckQuery?.TopLeft == null || foodtruckQuery?.BottomRight == null)
                return BadRequest();
            using (await Transaction())
            {
                var foodTrucksResult = await _foodtruckService.FindFoodTrucksWithin(foodtruckQuery.TopLeft, foodtruckQuery.BottomRight);
                var presencesResult = await _presenceService.FindPresencesWithin(foodtruckQuery.TopLeft, foodtruckQuery.BottomRight);
                return foodTrucksResult.Concat(await _foodtruckService.FindById(presencesResult.Select(p => p.FoodTruckId))).ToResult();
            }
        }
    }
}