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
using Presence = Persistency.Dtos.Presence;

namespace WebApplication.Controllers.FoodtruckStaff
{
    [Authorize(Roles = FoodtruckerRole.FoodtruckStaff)]
    [Route("api/foodtruckStaff/[controller]")]
    public class PresenceController : BaseController
    {
        private static readonly IImmutableSet<OwnershipType?> _canManipulateOwnerships =
            ImmutableHashSet.Create<OwnershipType?>(OwnershipType.ADMIN, OwnershipType.OWNER, OwnershipType.REPORTER);

        private readonly IFoodtruckOwnershipService _foodtruckOwnershipService;
        private readonly IPresenceService _presenceService;

        public PresenceController(IPresenceService presenceService,
            IFoodtruckOwnershipService foodtruckOwnershipService,
            UserManager<FoodtruckerUser> userManager,
            IPersistencyContext persistencyContext) : base(userManager, persistencyContext)
        {
            _presenceService = presenceService;
            _foodtruckOwnershipService = foodtruckOwnershipService;
        }

        private static bool ValidateCreateModifyPresence(CreateModifyPresence createModifyPresence) =>
            createModifyPresence?.Title == null ||
            createModifyPresence.Description == null;

        [HttpPost("{foodtruckSlug}")]
        public async Task<ActionResult<ActionResult<ResponseWithStatus<Presence>>>> CreatePresence([FromRoute] string foodtruckSlug, [FromBody] CreateModifyPresence createModifyPresence)
        {
            if (ValidateCreateModifyPresence(createModifyPresence))
                return BadRequest();

            Presence result;
            using (var transaction = await Transaction())
            {
                try
                {
                    var user = await CurrentUser();
                    if (!_canManipulateOwnerships.Contains(await _foodtruckOwnershipService.FindTypeByUserAndFoodtruck(user.Id, foodtruckSlug)))
                        return Forbid();
                    result = await _presenceService.CreatePresence(foodtruckSlug, createModifyPresence);
                    transaction.Commit();
                }
                catch (ValidationException<Presence> ex)
                {
                    transaction.Rollback();
                    return UnprocessableEntity(new ResponseWithStatus<Presence>
                    {
                        Dto = ex.Dto,
                        Successful = false,
                        Description = "Conflict with"
                    });
                }
            }

            return Ok(new ResponseWithStatus<Presence>
            {
                Dto = result,
                Successful = true
            });
        }

        [HttpPut("{presenceId}")]
        public async Task<ActionResult<ResponseWithStatus<Presence>>> ModifyPresence([FromRoute] Guid presenceId, [FromBody] CreateModifyPresence createModifyPresence)
        {
            if (ValidateCreateModifyPresence(createModifyPresence))
                return BadRequest();

            Presence result;
            using (var transaction = await Transaction())
            {
                try
                {
                    var user = await CurrentUser();
                    if (!_canManipulateOwnerships.Contains(await _foodtruckOwnershipService.FindTypeByUserAndPresence(user.Id, presenceId)))
                        return Forbid();
                    result = await _presenceService.ModifyPresence(presenceId, createModifyPresence);
                    transaction.Commit();
                }
                catch (ValidationException<Presence> ex)
                {
                    transaction.Rollback();
                    return UnprocessableEntity(new ResponseWithStatus<Presence>
                    {
                        Dto = ex.Dto,
                        Successful = false,
                        Description = "Conflict with"
                    });
                }
            }

            return Ok(new ResponseWithStatus<Presence>
            {
                Dto = result,
                Successful = true
            });
        }

        [HttpDelete("{presenceId}")]
        public async Task<ActionResult> DeletePresence([FromRoute] Guid presenceId)
        {
            using (var transaction = await Transaction())
            {
                var user = await CurrentUser();
                if (!_canManipulateOwnerships.Contains(await _foodtruckOwnershipService.FindTypeByUserAndPresence(user.Id, presenceId)))
                    return Forbid();
                await _presenceService.RemoveById(presenceId);
                transaction.Commit();
            }

            return Ok();
        }
    }
}