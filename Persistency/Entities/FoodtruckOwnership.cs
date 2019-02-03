using System;
using System.ComponentModel.DataAnnotations;

namespace Persistency.Entities
{
    public enum OwnershipType
    {
        OWNER = 0,
        ADMIN = 1,
        REPORTER = 2
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