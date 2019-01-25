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
        public async Task<ActionResult<FoodtrucksWithinResult>> Find([FromBody] FoodtrucksQuery foodtrucksQuery)
        {
            if (foodtrucksQuery?.TopLeft == null || foodtrucksQuery?.BottomRight == null)
                return BadRequest();
            using (await Transaction())
                return Ok(new FoodtrucksWithinResult
                {
                    Foodtrucks = await _foodtruckService.FindFoodTrucksWithin(foodtrucksQuery.TopLeft, foodtrucksQuery.BottomRight),
                    Presences = await _presenceService.FindPresencesWithin(foodtrucksQuery.TopLeft, foodtrucksQuery.BottomRight)
                });
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<GenericListResult<Foodtruck>>> FindByIds([FromBody] IdsQuery ids)
        {
            if (ids?.Ids == null)
                return BadRequest();
            return Ok(new GenericListResult<Foodtruck>
            {
                Result = await _foodtruckService.FindById(ids.Ids)
            });
        }
    }
}