using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistency;
using Persistency.Dtos;
using Persistency.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class PresenceController : BaseController
    {
        private readonly IPresenceService _presenceService;

        public PresenceController(IPresenceService presenceService,
            UserManager<Persistency.Entities.FoodtruckerUser> userManager,
            IPersistencyContext persistencyContext) : base(userManager, persistencyContext)
        {
            _presenceService = presenceService;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<IDictionary<Guid, IList<Presence>>>> Find([FromBody] PresencesQuery presencesQuery)
        {
            if (presencesQuery.Slugs == null)
                return BadRequest();
            return new OkObjectResult(await _presenceService.FindPresences(presencesQuery.Slugs));
        }
    }
}