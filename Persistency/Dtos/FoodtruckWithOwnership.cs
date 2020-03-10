using Persistency.Entities;

namespace Persistency.Dtos
{
    public sealed class FoodtruckWithOwnership
    {
        public FoodtruckDetailed Foodtruck { get; set; } = null!;
        public OwnershipType Type { get; set; }
    }
}