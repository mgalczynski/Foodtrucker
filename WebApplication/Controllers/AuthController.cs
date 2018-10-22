using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Dtos;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult<RegisterResult>> Register([FromBody] RegisterUser registerUser)
        {
            if (registerUser?.Email == null || registerUser?.Password == null)
                return BadRequest();
            var user = new IdentityUser {UserName = registerUser.Email, Email = registerUser.Email};
            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                return new RegisterResult {Successful = true};
            }
            else
            {
                return new RegisterResult
                    {Successful = false, Errors = result.Errors.Select(error => error.Description).ToList()};
            }
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginUser loginUser)
        {
            if (loginUser?.Email == null || loginUser?.Password == null)
                return BadRequest();
            return
                Mapper.Map<LoginResult>(
                    await _signInManager.PasswordSignInAsync(
                        loginUser.Email,
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