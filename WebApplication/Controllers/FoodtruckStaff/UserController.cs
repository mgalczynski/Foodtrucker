using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistency;
using Persistency.Dtos;
using Persistency.Entities;
using Persistency.Services;

namespace WebApplication.Controllers.FoodtruckStaff
{
    [Authorize(Roles = FoodtruckerRole.FoodtruckStaff)]
    [Route("api/foodtruckStaff/[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService,
            UserManager<FoodtruckerUser> userManager,
            IPersistencyContext persistencyContext) : base(userManager, persistencyContext)
        {
            _userService = userService;
        }


        [HttpPost("[action]")]
        public async Task<ActionResult<GenericListResult<User>>> Find([FromBody] FindUserArg arg)
        {
            using (await Transaction())
            {
                if (arg.Query == null)
                    return BadRequest();
                var query = arg.Query.Split().Where(a => a.Length >= 3).ToList();
                if (!query.Any() || arg.Except == null)
                    return BadRequest();
                return new GenericListResult<User>
                {
                    Result = await _userService.FindByQuery(query,
                        arg.Except.Concat(new[] {(await CurrentUser()).Email}).Distinct()
                    )
                };
            }
        }
    }
}