using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistency;
using Persistency.Dtos;
using Persistency.Entities;
using Persistency.Services;
using PresenceOrUnavailability = Persistency.Dtos.PresenceOrUnavailability;

namespace WebApplication.Controllers.FoodtruckStaff
{
    [Authorize(Roles = FoodtruckerRole.FoodtruckStaff)]
    [Route("api/foodtruckStaff/[controller]")]
    public class PresenceOrUnavailabilityController : BaseController
    {
        private static readonly IImmutableSet<OwnershipType?> _canManipulateOwnerships =
            ImmutableHashSet.Create<OwnershipType?>(OwnershipType.ADMIN, OwnershipType.OWNER, OwnershipType.REPORTER);

        private readonly IFoodtruckOwnershipService _foodtruckOwnershipService;
        private readonly IPresenceOrUnavailabilityService _presenceService;

        public PresenceOrUnavailabilityController(IPresenceOrUnavailabilityService presenceService,
            IFoodtruckOwnershipService foodtruckOwnershipService,
            UserManager<FoodtruckerUser> userManager,
            IPersistencyContext persistencyContext) : base(userManager, persistencyContext)
        {
            _presenceService = presenceService;
            _foodtruckOwnershipService = foodtruckOwnershipService;
        }

        private static bool ValidateCreateModifyPresenceOrUnavailability(CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability) =>
            createModifyPresenceOrUnavailability?.Title == null ||
            createModifyPresenceOrUnavailability.Description == null;

        [HttpPost("{foodtruckSlug}")]
        public async Task<ActionResult<ActionResult<ResponseWithStatus<PresenceOrUnavailability>>>> CreatePresenceOrUnavailability([FromRoute] string foodtruckSlug, [FromBody] CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability)
        {
            if (ValidateCreateModifyPresenceOrUnavailability(createModifyPresenceOrUnavailability))
                return BadRequest();

            PresenceOrUnavailability result;
            using (var transaction = await Transaction())
            {
                try
                {
                    var user = await CurrentUser();
                    if (!_canManipulateOwnerships.Contains(await _foodtruckOwnershipService.FindTypeByUserAndFoodtruck(user.Id, foodtruckSlug)))
                        return Forbid();
                    result = await _presenceService.CreatePresenceOrUnavailability(foodtruckSlug, createModifyPresenceOrUnavailability);
                    transaction.Commit();
                }
                catch (ValidationException<PresenceOrUnavailability> ex)
                {
                    transaction.Rollback();
                    return UnprocessableEntity(new ResponseWithStatus<PresenceOrUnavailability>
                    {
                        Dto = ex.Dto,
                        Successful = false,
                        Description = "Conflict with"
                    });
                }
                catch (ArgumentException)
                {
                    return BadRequest();
                }
            }

            return Ok(result.MapToResponse());
        }

        [HttpPost("{foodtruckSlug}")]
        public async Task<ActionResult<ActionResult<ResponseWithStatus<PresenceOrUnavailability>>>> ValidatePresenceOrUnavailability([FromRoute] string foodtruckSlug, [FromBody] CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability)
        {
            if (ValidateCreateModifyPresenceOrUnavailability(createModifyPresenceOrUnavailability))
                return BadRequest();

            ResponseWithStatus<PresenceOrUnavailability> result;
            using (await Transaction())
            {
                try
                {
                    var user = await CurrentUser();
                    if (!_canManipulateOwnerships.Contains(await _foodtruckOwnershipService.FindTypeByUserAndFoodtruck(user.Id, foodtruckSlug)))
                        return Forbid();
                    result = await _presenceService.ValidatePresenceOrUnavailability(foodtruckSlug, createModifyPresenceOrUnavailability);
                }
                catch (ArgumentException)
                {
                    return BadRequest();
                }
            }

            return result.Successful ? Ok(result) : UnprocessableEntity(result);
        }

        [HttpPost("{foodtruckSlug}/{presenceId}")]
        public async Task<ActionResult<ActionResult<ResponseWithStatus<PresenceOrUnavailability>>>> ValidatePresenceOrUnavailability([FromRoute] Guid presenceId, [FromBody] CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability)
        {
            if (ValidateCreateModifyPresenceOrUnavailability(createModifyPresenceOrUnavailability))
                return BadRequest();

            ResponseWithStatus<PresenceOrUnavailability> result;
            using (var transaction = await Transaction())
            {
                try
                {
                    var user = await CurrentUser();
                    if (!_canManipulateOwnerships.Contains(await _foodtruckOwnershipService.FindTypeByUserAndPresenceOrUnavailability(user.Id, presenceId)))
                        return Forbid();
                    result = await _presenceService.ValidatePresenceOrUnavailability(presenceId, createModifyPresenceOrUnavailability);
                    transaction.Commit();
                }
                catch (ValidationException<PresenceOrUnavailability> ex)
                {
                    transaction.Rollback();
                    return UnprocessableEntity(ex.MapToResponse());
                }
            }

            return result.Successful ? Ok(result) : UnprocessableEntity(result);
        }

        [HttpPut("{presenceId}")]
        public async Task<ActionResult<ResponseWithStatus<PresenceOrUnavailability>>> ModifyPresenceOrUnavailability([FromRoute] Guid presenceId, [FromBody] CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability)
        {
            if (ValidateCreateModifyPresenceOrUnavailability(createModifyPresenceOrUnavailability))
                return BadRequest();

            PresenceOrUnavailability result;
            using (var transaction = await Transaction())
            {
                try
                {
                    var user = await CurrentUser();
                    if (!_canManipulateOwnerships.Contains(await _foodtruckOwnershipService.FindTypeByUserAndPresenceOrUnavailability(user.Id, presenceId)))
                        return Forbid();
                    result = await _presenceService.ModifyPresenceOrUnavailability(presenceId, createModifyPresenceOrUnavailability);
                    transaction.Commit();
                }
                catch (ValidationException<PresenceOrUnavailability> ex)
                {
                    transaction.Rollback();
                    return UnprocessableEntity(ex.MapToResponse());
                }
            }

            return Ok(new ResponseWithStatus<PresenceOrUnavailability>
            {
                Dto = result,
                Successful = true
            });
        }

        [HttpDelete("{presenceId}")]
        public async Task<ActionResult> DeletePresenceOrUnavailability([FromRoute] Guid presenceId)
        {
            using (var transaction = await Transaction())
            {
                var user = await CurrentUser();
                if (!_canManipulateOwnerships.Contains(await _foodtruckOwnershipService.FindTypeByUserAndPresenceOrUnavailability(user.Id, presenceId)))
                    return Forbid();
                await _presenceService.RemoveById(presenceId);
                transaction.Commit();
            }

            return Ok();
        }
    }
}