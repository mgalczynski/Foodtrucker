using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistency;
using Persistency.Entities;
using WebApplication.Dtos;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<FoodtruckerUser> _userManager;
        private readonly SignInManager<FoodtruckerUser> _signInManager;
        private readonly RoleManager<FoodtruckerRole> _roleManager;
        private readonly IPersistencyContext _persistencyContext;

        public AuthController(UserManager<FoodtruckerUser> userManager, SignInManager<FoodtruckerUser> signInManager,
            RoleManager<FoodtruckerRole> roleManager, IPersistencyContext persistencyContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _persistencyContext = persistencyContext;
        }

        private async Task<ActionResult<RegisterResult>> Register(FoodtruckerUser user, string password,
            string role)
        {
            if (string.IsNullOrEmpty(user.Email) || password == null ||
                string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
                return BadRequest();
            user.Active = role != FoodtruckerRole.ServiceStaff;
            using (var transaction = await _persistencyContext.Database.BeginTransactionAsync())
            {
                var result = await _userManager.CreateAsync(user, password);
                var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
                if (!addToRoleResult.Succeeded)
                {
                    transaction.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                if (result.Succeeded)
                {
                    if (role != FoodtruckerRole.ServiceStaff)
                        await _signInManager.SignInAsync(user, isPersistent: true);
                    return new RegisterResult {Successful = true};
                }
                else
                {
                    return new RegisterResult
                        {Successful = false, Errors = result.Errors.Select(error => error.Description).ToList()};
                }
            }
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult<RegisterResult>> Register([FromBody] RegisterUser registerUser)
        {
            var user = new FoodtruckerUser
            {
                UserName = registerUser?.Email?.Trim(),
                Email = registerUser?.Email?.Trim(),
                LastName = registerUser?.LastName?.Trim(),
                FirstName = registerUser?.FirstName?.Trim()
            };
            return await Register(user, registerUser?.Password, FoodtruckerRole.Customer);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult<RegisterResult>> RegisterFoodTruckUser([FromBody] RegisterUser registerUser)
        {
            var user = new FoodtruckerUser
            {
                UserName = registerUser?.Email?.Trim(),
                Email = registerUser?.Email?.Trim(),
                LastName = registerUser?.LastName?.Trim(),
                FirstName = registerUser?.FirstName?.Trim()
            };
            return await Register(user, registerUser?.Password, FoodtruckerRole.FoodtruckStaff);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginUser loginUser)
        {
            if (loginUser?.Email == null || loginUser?.Password == null)
                return BadRequest();
            var user = await _userManager.FindByEmailAsync(loginUser.Email);
            return !user.Active
                ? new LoginResult
                {
                    IsNotAllowed = true,
                }
                : Mapper.Map<LoginResult>(await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    loginUser.Password,
                    loginUser.RememberMe,
                    false
                ));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}