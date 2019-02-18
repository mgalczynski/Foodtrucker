using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Persistency;
using Persistency.Entities;

namespace WebApplication.Controllers
{
    [Authorize]
    [ApiController]
    public class BaseController : Controller
    {
        private readonly IPersistencyContext _persistencyContext;

        protected BaseController(UserManager<FoodtruckerUser> userManager, IPersistencyContext persistencyContext)
        {
            UserManager = userManager;
            _persistencyContext = persistencyContext;
        }

        protected async Task<FoodtruckerUser> CurrentUser() =>
            await UserManager.FindByNameAsync(User.Identity.Name);

        protected UserManager<FoodtruckerUser> UserManager { get; }

        protected async Task<IDbContextTransaction> Transaction() =>
            await _persistencyContext.Database.BeginTransactionAsync();

        protected ActionResult<T> UnprocessableEntity<T>(T value) =>
            StatusCode((int) HttpStatusCode.UnprocessableEntity, value);
    }
}