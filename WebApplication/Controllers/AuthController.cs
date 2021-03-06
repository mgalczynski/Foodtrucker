using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistency;
using Persistency.Dtos;
using Persistency.Entities;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IRuntimeMapper _runtimeMapper;
        private readonly IPersistencyContext _persistencyContext;
        private readonly SignInManager<FoodtruckerUser> _signInManager;
        private readonly UserManager<FoodtruckerUser> _userManager;

        public AuthController(IRuntimeMapper runtimeMapper, UserManager<FoodtruckerUser> userManager, SignInManager<FoodtruckerUser> signInManager, IPersistencyContext persistencyContext)
        {
            _runtimeMapper = runtimeMapper;
            _userManager = userManager;
            _signInManager = signInManager;
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
                    return new RegisterResult {Successful = false, Errors = result.Errors.Select(error => error.Description).ToList()};

                var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
                if (!addToRoleResult.Succeeded)
                {
                    transaction.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                if (role != FoodtruckerRole.ServiceStaff)
                    await _signInManager.SignInAsync(user, isPersistent: true);
                transaction.Commit();
            }

            return new RegisterResult {Successful = true, User = _runtimeMapper.Map<User>(user), Roles = await _userManager.GetRolesAsync(user)};
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
        public async Task<ActionResult<RegisterResult>> RegisterStaff([FromBody] RegisterUser registerUser)
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
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginUser loginUser) =>
            await Login(loginUser, FoodtruckerRole.Customer);

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult<LoginResult>> LoginStaff([FromBody] LoginUser loginUser) =>
            await Login(loginUser, FoodtruckerRole.FoodtruckStaff);

        private async Task<ActionResult<LoginResult>> Login([FromBody] LoginUser loginUser, string role)
        {
            if (loginUser?.Email == null || loginUser?.Password == null)
                return BadRequest();
            var user = await _userManager.FindByEmailAsync(loginUser.Email);
            if (user == null || !await _userManager.IsInRoleAsync(user, role))
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
            var result = new LoginResult
            {
                Successful = signInResult.Succeeded,
                IsLockedOut = signInResult.IsLockedOut,
                IsNotAllowed = signInResult.IsNotAllowed
            };
            if (signInResult.Succeeded)
            {
                result.User = _runtimeMapper.Map<User>(user);
                result.Roles = await _userManager.GetRolesAsync(user);
            }

            return result;
        }

        private async Task<FoodtruckerUser> FoodtruckerUser()
        {
            var username = User.Identity.Name;
            return username == null ? null : await _userManager.FindByNameAsync(username);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePassword changePassword)
        {
            IdentityResult result;
            using (var transaction = await _persistencyContext.Database.BeginTransactionAsync())
            {
                result = await _userManager.ChangePasswordAsync(await FoodtruckerUser(), changePassword.CurrentPassword, changePassword.NewPassword);

                transaction.Commit();
            }

            return result.Succeeded ? (ActionResult) Ok() : BadRequest(new Result {Reason = string.Join(", ", result.Errors.Select(e => e.Description))});
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<CheckResult> Check()
        {
            var user = await FoodtruckerUser();
            return user == null
                ? new CheckResult {IsSignedIn = false}
                : new CheckResult {IsSignedIn = true, User = _runtimeMapper.Map<User>(user), Roles = await _userManager.GetRolesAsync(user)};
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}