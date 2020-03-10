using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class FoodtrucksWithinResult
    {
        public IList<Foodtruck> Foodtrucks { get; set; } = null!;
        public IList<PresenceOrUnavailability> PresencesOrUnavailabilities { get; set; } = null!;
    }
}