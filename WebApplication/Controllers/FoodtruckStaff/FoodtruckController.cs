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
        private readonly IFoodtruckOwnershipService _foodtruckOwnershipService;
        private readonly IFoodtruckService _foodtruckService;

        public FoodtruckController(IFoodtruckService foodtruckService,
            IFoodtruckOwnershipService foodtruckOwnershipService,
            UserManager<FoodtruckerUser> userManager,
            IPersistencyContext persistencyContext) : base(userManager, persistencyContext)
        {
            _foodtruckService = foodtruckService;
            _foodtruckOwnershipService = foodtruckOwnershipService;
        }

        private static bool ValidateCreateModifyFoodtruck(CreateModifyFoodtruck createModifyFoodtruck) =>
            createModifyFoodtruck?.Name == null || createModifyFoodtruck.DisplayName == null;

        [HttpPost]
        public async Task<ActionResult<FoodtruckDetailed>> CreateNewFoodtruck([FromBody] CreateModifyFoodtruck createNewFoodtruck)
        {
            if (ValidateCreateModifyFoodtruck(createNewFoodtruck))
                return BadRequest();
            FoodtruckDetailed foodtruck;
            using (var transaction = await Transaction())
            {
                var user = await CurrentUser();
                foodtruck = await _foodtruckService.CreateNewFoodtruck(createNewFoodtruck);
                await _foodtruckOwnershipService.CreateOwnership(user.Id, foodtruck.Slug,
                    OwnershipType.OWNER);
                transaction.Commit();
            }

            return foodtruck;
        }


        [HttpPut("{foodtruckSlug}")]
        public async Task<ActionResult<FoodtruckDetailed>> ModifyFoodtruck(
            [FromRoute] string foodtruckSlug,
            [FromBody] CreateModifyFoodtruck modifyFoodtruck
        )
        {
            if (ValidateCreateModifyFoodtruck(modifyFoodtruck))
                return BadRequest();
            FoodtruckDetailed foodtruck;
            using (var transaction = await Transaction())
            {
                var currentUser = await CurrentUser();
                if (!await _foodtruckOwnershipService.CanManipulate(currentUser.Id, foodtruckSlug, OwnershipType.ADMIN))
                    return Forbid();
                foodtruck = await _foodtruckService.ModifyFoodtruck(foodtruckSlug, modifyFoodtruck);
                transaction.Commit();
            }

            return foodtruck;
        }

        [HttpDelete("{foodtruckSlug}")]
        public async Task<ActionResult> DeleteFoodtruck([FromRoute] string foodtruckSlug)
        {
            using (var transaction = await Transaction())
            {
                var user = await CurrentUser();
                if (await _foodtruckOwnershipService.FindTypeByUserAndFoodtruck(user.Id, foodtruckSlug) != OwnershipType.OWNER)
                    return Forbid();
                await _foodtruckService.MarkAsDeleted(foodtruckSlug);
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
                if (!await _foodtruckOwnershipService.CanManipulate((await CurrentUser()).Id, foodtruck.Slug,
                    createNewOwnership.Type))
                    return Forbid();
                await _foodtruckOwnershipService.CreateOwnership((await taskUser).Id, foodtruck.Slug,
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

                var ownership = await _foodtruckOwnershipService.FindByUserEmailAndFoodtruck(deleteOwnership.Email, foodtruck.Slug);
                if (ownership == null || !await _foodtruckOwnershipService.CanManipulate(currentUser.Id, foodtruck.Slug, ownership.Type))
                    return Forbid();
                await _foodtruckOwnershipService.DeleteOwnership(deleteOwnership.Email, foodtruck.Slug);
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

                var ownership = await _foodtruckOwnershipService.FindByUserEmailAndFoodtruck(changeOwnership.Email, foodtruck.Slug);
                if (ownership == null || !await _foodtruckOwnershipService.CanManipulate(currentUser.Id, foodtruck.Slug, Min(ownership.Type, changeOwnership.Type)))
                    return Forbid();
                await _foodtruckOwnershipService.ChangeOwnership(foodtruck.Slug, changeOwnership.Email, changeOwnership.Type);
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

        private static OwnershipType Min(params OwnershipType[] args) =>
            (OwnershipType) args.Cast<int>().Min();
    }
}