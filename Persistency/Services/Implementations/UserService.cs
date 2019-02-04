using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistency.Dtos;
using Entity = Persistency.Entities.FoodtruckerUser;
using FoodtruckerRole = Persistency.Entities.FoodtruckerRole;

namespace Persistency.Services.Implementations
{
    internal class UserService : IUserService
    {
        private readonly IInternalPersistencyContext _persistencyContext;
        private readonly RoleManager<FoodtruckerRole> _roleManager;

        public UserService(IInternalPersistencyContext persistencyContext, RoleManager<FoodtruckerRole> roleManager)
        {
            _persistencyContext = persistencyContext;
            _roleManager = roleManager;
        }

        private async Task<IList<Entity>> FindUserByMails(IList<string> mails) =>
            await _persistencyContext.Users.Where(u => mails.Contains(u.Email)).ToListAsync();

        public async Task<IList<User>> FindByQuery(IEnumerable<string> args, IEnumerable<string> emails)
        {
            var argsList = args.Select(a => a.ToLower()).ToList();
            var exceptList = (await FindUserByMails(emails.Distinct().ToList())).Select(u => u.Id);
            return await _persistencyContext.Users.FromSql(
                    $@"SELECT u.*
                       FROM ""AspNetUserRoles"" ur
                            INNER JOIN ""AspNetUsers"" u ON ur.""UserId"" = u.""Id""
                            INNER JOIN ""AspNetRoles"" r ON ur.""RoleId"" = r.""Id""
                       WHERE r.""Name"" = {_roleManager.NormalizeKey(FoodtruckerRole.FoodtruckStaff)}"
                )
                .Where(u => !exceptList.Contains(u.Id) &&
                            argsList.Any(a => u.FirstName.ToLower().Contains(a) ||
                                              u.LastName.ToLower().Contains(a) ||
                                              u.Email.ToLower().Contains(a))
                )
                .ProjectToListAsync<User>();
        }
    }
}