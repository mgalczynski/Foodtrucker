using Persistency.Entities;

namespace Persistency.Dtos
{
    public sealed class FoodtruckWithOwnership
    {
        public FoodtruckDetailed Foodtruck { get; set; }
        public OwnershipType Type { get; set; }
    }
}