using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistency;
using Persistency.Dtos;
using Persistency.Entities;
using Persistency.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class FoodtruckController : BaseController
    {
        private readonly IFoodtruckService _foodtruckService;
        private readonly IFoodtruckOwnershipService _foodtruckOwnershipService;

        public FoodtruckController(IFoodtruckService foodtruckService,
            IFoodtruckOwnershipService foodtruckOwnershipService,
            UserManager<FoodtruckerUser> userManager,
            IPersistencyContext persistencyContext) : base(userManager, persistencyContext)
        {
            _foodtruckService = foodtruckService;
            _foodtruckOwnershipService = foodtruckOwnershipService;
        }

        [HttpPost]
        public async Task<ActionResult<InsertStatus<Guid>>> CreateNewFoodtruck(
            [FromBody] CreateNewFoodtruck createNewFoodtruck)
        {
            if (createNewFoodtruck?.Name == null || createNewFoodtruck?.DisplayName == null)
                return BadRequest();
            Guid foodtruckId;
            using (var transaction = await Transaction())
            {
                var taskUser = CurrentUser();
                foodtruckId = await _foodtruckService.CreateNewFoodtruck(createNewFoodtruck);
                await _foodtruckOwnershipService.CreateOwnership((await taskUser).Id, foodtruckId,
                    OwnershipType.OWNER);
                transaction.Commit();
            }

            return new InsertStatus<Guid> {Id = foodtruckId, Successful = true};
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteFoodtruck([FromRoute] Guid id)
        {
            using (var transaction = await Transaction())
            {
                var user = await CurrentUser();
                if (await _foodtruckOwnershipService.FindTypeByUserAndFoodtruck(user.Id, id) != OwnershipType.OWNER)
                    return Forbid();
                await _foodtruckService.MarkAsDeleted(id);
                transaction.Commit();
            }
            return Ok();
        }
    }
}