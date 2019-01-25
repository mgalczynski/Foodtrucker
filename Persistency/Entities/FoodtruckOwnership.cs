using System;
using System.ComponentModel.DataAnnotations;

namespace Persistency.Entities
{
    public enum OwnershipType
    {
        OWNER,
        ADMIN,
        REPORTER
    }

    public class FoodtruckOwnership
    {
        public OwnershipType Type { get; set; }
        [Required] public Guid? UserId { get; set; }
        public FoodtruckerUser User { get; set; }
        [Required] public Guid? FoodtruckId { get; set; }
        public Foodtruck Foodtruck { get; set; }
    }
}