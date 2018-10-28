using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Persistency;
using Persistency.Entities;

namespace WebApplication.Controllers
{
    public class BaseController : Controller
    {
        private readonly UserManager<FoodtruckerUser> _userManager;
        private readonly IPersistencyContext _persistencyContext;

        protected BaseController(UserManager<FoodtruckerUser> userManager, IPersistencyContext persistencyContext)
        {
            _userManager = userManager;
            _persistencyContext = persistencyContext;
        }

        protected async Task<FoodtruckerUser> CurrentUser() =>
            await _userManager.FindByNameAsync(User.Identity.Name);

        protected async Task<IDbContextTransaction> Transaction() =>
            await _persistencyContext.Database.BeginTransactionAsync();
    }
}