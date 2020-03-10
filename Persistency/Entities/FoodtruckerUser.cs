using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Persistency.Entities
{
    public class FoodtruckerUser : IdentityUser<Guid>
    {
        [Required] public override string Email { get; set; } = null!;
        [Required] public override string NormalizedEmail { get; set; } = null!;
        [Required] public string FirstName { get; set; } = null!;
        [Required] public string LastName { get; set; } = null!;
        public bool Active { get; set; }
    }
}