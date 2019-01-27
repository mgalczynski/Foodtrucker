using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistency;
using Persistency.Dtos;
using Persistency.Entities;
using Persistency.Services;
using Foodtruck = Persistency.Dtos.Foodtruck;

namespace WebApplication.Controllers.FoodtruckStaff
{
    [Authorize(Roles = FoodtruckerRole.FoodtruckStaff)]
    [Route("api/foodtruckStaff/[controller]")]
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
        public async Task<ActionResult<Foodtruck>> CreateNewFoodtruck(
            [FromBody] CreateNewFoodtruck createNewFoodtruck)
        {
            if (createNewFoodtruck?.Name == null || createNewFoodtruck?.DisplayName == null)
                return BadRequest();
            Foodtruck foodtruck;
            using (var transaction = await Transaction())
            {
                var taskUser = CurrentUser();
                foodtruck = await _foodtruckService.CreateNewFoodtruck(createNewFoodtruck);
                await _foodtruckOwnershipService.CreateOwnership((await taskUser).Id, foodtruck.Id,
                    OwnershipType.OWNER);
                transaction.Commit();
            }

            return foodtruck;
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

        [HttpPost("{foodtruckId}/[action]")]
        public async Task<ActionResult> CreateNewOwnership([FromRoute] Guid foodtruckId,
            [FromBody] CreateNewOwnership createNewOwnership)
        {
            if (createNewOwnership?.Email == null)
                return BadRequest();
            using (var transaction = await Transaction())
            {
                var taskUser = UserManager.FindByEmailAsync(createNewOwnership.Email);
                if (!await _foodtruckOwnershipService.CanManipulate((await CurrentUser()).Id, foodtruckId,
                    createNewOwnership.Type))
                    return Forbid();
                await _foodtruckOwnershipService.CreateOwnership((await taskUser).Id, foodtruckId,
                    createNewOwnership.Type);
                transaction.Commit();
            }

            return Ok();
        }

        [HttpPost("{foodtruckId}/[action]")]
        public async Task<ActionResult> DeleteOwnership([FromRoute] Guid foodtruckId,
            [FromBody] DeleteOwnership deleteOwnership)
        {
            if (deleteOwnership?.Email == null)
                return BadRequest();
            using (var transaction = await Transaction())
            {
                var taskCurrentUser = CurrentUser();
                var ownership =
                    await _foodtruckOwnershipService.FindByUserEmailAndFoodtruck(deleteOwnership.Email, foodtruckId);
                if (!await _foodtruckOwnershipService.CanManipulate((await taskCurrentUser).Id, foodtruckId,
                    ownership.Type))
                    return Forbid();
                await _foodtruckOwnershipService.DeleteOwnership(deleteOwnership.Email, foodtruckId);
                transaction.Commit();
            }

            return Ok();
        }
    }
}