using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Persistency.Entities
{
    public class FoodtruckerUser : IdentityUser<Guid>
    {
        [Required] public override string Email { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        public bool Active { get; set; }
    }
}