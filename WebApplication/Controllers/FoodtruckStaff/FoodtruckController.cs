using System;
using System.Linq;
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

        [HttpDelete("{foodtruckId}")]
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

        [HttpPost("{foodtruckSlug}/[action]")]
        public async Task<ActionResult> CreateNewOwnership([FromRoute] string foodtruckSlug,
            [FromBody] CreateNewOwnership createNewOwnership)
        {
            if (createNewOwnership?.Email == null)
                return BadRequest();
            using (var transaction = await Transaction())
            {
                var foodtruck = await _foodtruckService.FindBySlug(foodtruckSlug);
                if (foodtruck == null)
                    return NotFound();
                var taskUser = UserManager.FindByEmailAsync(createNewOwnership.Email);
                if (!await _foodtruckOwnershipService.CanManipulate((await CurrentUser()).Id, foodtruck.Id,
                    createNewOwnership.Type))
                    return Forbid();
                await _foodtruckOwnershipService.CreateOwnership((await taskUser).Id, foodtruck.Id,
                    createNewOwnership.Type);
                transaction.Commit();
            }

            return Ok();
        }

        [HttpPost("{foodtruckSlug}/[action]")]
        public async Task<ActionResult> DeleteOwnership([FromRoute] string foodtruckSlug,
            [FromBody] DeleteOwnership deleteOwnership)
        {
            if (deleteOwnership?.Email == null)
                return BadRequest();
            using (var transaction = await Transaction())
            {
                var foodtruck = await _foodtruckService.FindBySlug(foodtruckSlug);
                if (foodtruck == null)
                    return NotFound();
                var currentUser = await CurrentUser();
                if (currentUser.Email == deleteOwnership.Email)
                    return Forbid();

                var ownership = await _foodtruckOwnershipService.FindByUserEmailAndFoodtruck(deleteOwnership.Email, foodtruck.Id);
                if (ownership == null || !await _foodtruckOwnershipService.CanManipulate(currentUser.Id, foodtruck.Id, ownership.Type))
                    return Forbid();
                await _foodtruckOwnershipService.DeleteOwnership(deleteOwnership.Email, foodtruck.Id);
                transaction.Commit();
            }

            return Ok();
        }

        [HttpPost("{foodtruckSlug}/[action]")]
        public async Task<ActionResult> ChangeOwnership([FromRoute] string foodtruckSlug,
            [FromBody] ChangeOwnership changeOwnership)
        {
            if (changeOwnership?.Email == null)
                return BadRequest();
            using (var transaction = await Transaction())
            {
                var foodtruck = await _foodtruckService.FindBySlug(foodtruckSlug);
                if (foodtruck == null)
                    return NotFound();
                var currentUser = await CurrentUser();
                if (currentUser.Email == changeOwnership.Email)
                    return Forbid();

                var ownership = await _foodtruckOwnershipService.FindByUserEmailAndFoodtruck(changeOwnership.Email, foodtruck.Id);
                if (ownership == null || !await _foodtruckOwnershipService.CanManipulate(currentUser.Id, foodtruck.Id, Min(ownership.Type, changeOwnership.Type)))
                    return Forbid();
                await _foodtruckOwnershipService.ChangeOwnership(foodtruck.Id, changeOwnership.Email, changeOwnership.Type);
                transaction.Commit();
            }

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<GenericListResult<FoodtruckWithOwnership>>> GetFoodtrucks()
        {
            using (await Transaction())
                return Ok(new GenericListResult<FoodtruckWithOwnership>
                {
                    Result = await _foodtruckOwnershipService.FindFoodtruckOwnershipsByUser((await CurrentUser()).Id)
                });
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<FoodtruckDetailed>> FindBySlug([FromRoute] string slug)
        {
            var result = await _foodtruckService.FindBySlugDetailed(slug);
            if (result == null || !result.Ownerships.Select(o => o.User.Email).Contains((await CurrentUser()).Email))
                return NotFound();
            return result;
        }

        public static OwnershipType Min(params OwnershipType[] args) => 
            (OwnershipType) args.Cast<int>().Min();
    }
}