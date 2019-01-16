using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistency;
using Persistency.Entities;
using Persistency.Dtos;

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

        private async Task<ActionResult<RegisterResult>> Register(FoodtruckerUser user, string password, string role)
        {
            if (string.IsNullOrEmpty(user.Email) || password == null ||
                string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
                return BadRequest();
            user.Active = role != FoodtruckerRole.ServiceStaff;
            using (var transaction = await _persistencyContext.Database.BeginTransactionAsync())
            {
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    return new RegisterResult
                        {Successful = false, Errors = result.Errors.Select(error => error.Description).ToList()};

                var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
                if (!addToRoleResult.Succeeded)
                {
                    transaction.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                if (role != FoodtruckerRole.ServiceStaff)
                    await _signInManager.SignInAsync(user, isPersistent: true);
                return new RegisterResult {Successful = true, User = Mapper.Map<User>(user)};
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
                FirstName = registerUser?.FirstName?.Trim(),
                SecurityStamp = Guid.NewGuid().ToString()
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
                FirstName = registerUser?.FirstName?.Trim(),
                SecurityStamp = Guid.NewGuid().ToString()
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
            if (user == null)
                return new LoginResult
                {
                    IsNotAllowed = false,
                    Successful = false,
                    IsLockedOut = false
                };
            if (!user.Active)
                return new LoginResult
                {
                    IsNotAllowed = true,
                    Successful = false,
                    IsLockedOut = false
                };

            var signInResult = await _signInManager.PasswordSignInAsync(
                user,
                loginUser.Password,
                loginUser.RememberMe,
                false
            );
            return new LoginResult
            {
                User = signInResult.Succeeded ? Mapper.Map<User>(user) : null,
                Successful = signInResult.Succeeded,
                IsLockedOut = signInResult.IsLockedOut,
                IsNotAllowed = signInResult.IsNotAllowed
            };
        }

        [HttpGet("[action]")]
        public async Task<CheckResult> Check()
        {
            var username = User.Identity.Name;
            if (username == null)
                return new CheckResult {IsSignedIn = false};
            var user = await _userManager.FindByNameAsync(username);
            return new CheckResult {IsSignedIn = true, User = Mapper.Map<User>(user)};
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}