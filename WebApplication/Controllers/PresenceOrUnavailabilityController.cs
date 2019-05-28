using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistency;
using Persistency.Dtos;
using Persistency.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class PresenceOrUnavailabilityController : BaseController
    {
        private readonly IPresenceOrUnavailabilityService _presenceService;

        public PresenceOrUnavailabilityController(IPresenceOrUnavailabilityService presenceService,
            UserManager<Persistency.Entities.FoodtruckerUser> userManager,
            IPersistencyContext persistencyContext) : base(userManager, persistencyContext)
        {
            _presenceService = presenceService;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult<IDictionary<Guid, IList<PresenceOrUnavailability>>>> Find([FromBody] PresencesOrUnavailabilitiesQuery presencesQuery)
        {
            if (presencesQuery.Slugs == null)
                return BadRequest();
            return new OkObjectResult(await _presenceService.FindPresencesOrUnavailabilities(presencesQuery.Slugs));
        }
    }
}