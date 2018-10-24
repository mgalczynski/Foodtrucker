using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistency.Entities;
using WebApplication.Dtos;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<FoodtruckerUser> _userManager;
        private readonly SignInManager<FoodtruckerUser> _signInManager;

        public AuthController(UserManager<FoodtruckerUser> userManager, SignInManager<FoodtruckerUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
            if (string.IsNullOrEmpty(user.Email) || registerUser?.Password == null ||
                string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
                return BadRequest();
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