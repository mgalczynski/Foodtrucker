using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Persistency.Entities;
using Persistency.Services.Implementations;
using Xunit;

namespace Persistency.Test.Services.Implementations
{
    public class UserServiceTest : BaseTests
    {
        private readonly UserService _userService;

        public UserServiceTest()
        {
            var roleManager = new RoleManager<FoodtruckerRole>(
                new RoleStore<FoodtruckerRole, AbstractPersistencyContext, Guid>(Context),
                new IRoleValidator<FoodtruckerRole>[] { },
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new NullLogger<RoleManager<FoodtruckerRole>>()
            );
            Persistency.CreateRoles(roleManager);
            var users = new List<FoodtruckerUser>
            {
                new FoodtruckerUser
                {
                    Email = "f.user@miroslawgalczynski.com",
                    FirstName = "First",
                    LastName = "User"
                },
                new FoodtruckerUser
                {
                    Email = "s.user@miroslawgalczynski.com",
                    FirstName = "Second",
                    LastName = "User"
                },
                new FoodtruckerUser
                {
                    Email = "t.user@miroslawgalczynski.com",
                    FirstName = "Third",
                    LastName = "User"
                },
                new FoodtruckerUser
                {
                    Email = "j.d@miroslawgalczynski.com",
                    FirstName = "John",
                    LastName = "Doe"
                }
            };
            users.ForEach(u =>
            {
                u.UserName = u.Email;
                u.NormalizedEmail = u.Email;
            });
            Context.Users.AddRange(users);
            Context.SaveChanges();
            var foodtruckStaff = Context.Roles.First(r => r.Name == FoodtruckerRole.FoodtruckStaff);
            Context.UserRoles.AddRange(Context.Users.Select(u => new IdentityUserRole<Guid>
            {
                RoleId = foodtruckStaff.Id,
                UserId = u.Id
            }));
            Context.SaveChanges();
            _userService = new UserService(Context, roleManager);
        }

        [Fact]
        public async void ShouldReturnAllUsersWithLastNameUser()
        {
            var result = await _userService.FindByQuery(new[] {"user"}, new string[] { });
            Assert.Equal(new HashSet<string>
            {
                "f.user@miroslawgalczynski.com",
                "s.user@miroslawgalczynski.com",
                "t.user@miroslawgalczynski.com"
            }, result.Select(u => u.Email).ToHashSet());
        }

        [Fact]
        public async void ShouldReturnAllUsersWithLastNameUserWhenUpperCase()
        {
            var result = await _userService.FindByQuery(new[] {"USER"}, new string[] { });
            Assert.Equal(new HashSet<string>
            {
                "f.user@miroslawgalczynski.com",
                "s.user@miroslawgalczynski.com",
                "t.user@miroslawgalczynski.com"
            }, result.Select(u => u.Email).ToHashSet());
        }

        [Fact]
        public async void ShouldReturnAllUsersWithLastNameUserWithoutFirstUser()
        {
            var result = await _userService.FindByQuery(new[] {"user"},
                new[] {"f.user@miroslawgalczynski.com"});
            Assert.Equal(new HashSet<string>
            {
                "s.user@miroslawgalczynski.com",
                "t.user@miroslawgalczynski.com"
            }, result.Select(u => u.Email).ToHashSet());
        }

        [Fact]
        public async void ShouldReturnJohn()
        {
            var result = await _userService.FindByQuery(new[] {"john"}, new string[] { });
            Assert.Equal(new HashSet<string>
            {
                "j.d@miroslawgalczynski.com"
            }, result.Select(u => u.Email).ToHashSet());
        }

        [Fact]
        public async void ShouldReturnDoe()
        {
            var result = await _userService.FindByQuery(new[] {"doe"}, new string[] { });
            Assert.Equal(new HashSet<string>
            {
                "j.d@miroslawgalczynski.com"
            }, result.Select(u => u.Email).ToHashSet());
        }

        [Fact]
        public async void ShouldReturnJohnWhenUpperCase()
        {
            var result = await _userService.FindByQuery(new[] {"JOHN"}, new string[] { });
            Assert.Equal(new HashSet<string>
            {
                "j.d@miroslawgalczynski.com"
            }, result.Select(u => u.Email).ToHashSet());
        }

        [Fact]
        public async void ShouldReturnDoeWhenUpperCase()
        {
            var result = await _userService.FindByQuery(new[] {"DOE"}, new string[] { });
            Assert.Equal(new HashSet<string>
            {
                "j.d@miroslawgalczynski.com"
            }, result.Select(u => u.Email).ToHashSet());
        }
    }
}