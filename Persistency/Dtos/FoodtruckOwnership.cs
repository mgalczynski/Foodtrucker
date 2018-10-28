using System;
using Persistency.Entities;

namespace Persistency.Dtos
{
    public class FoodtruckOwnership
    {
        public Guid FoodtruckId { get; set; }
        public string UserEmail { get; set; }
        public OwnershipType Type { get; set; }
    }
}