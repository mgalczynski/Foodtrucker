using Persistency.Entities;

namespace Persistency.Dtos
{
    public sealed class FoodtruckWithOwnership
    {
        public Foodtruck Foodtruck { get; set; }
        public OwnershipType Type { get; set; }
    }
}