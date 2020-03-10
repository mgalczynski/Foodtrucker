using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class FoodtruckWithPresencesOrUnavailabilies
    {
        public Foodtruck Foodtruck { get; set; } = null!;
        public IList<PresenceOrUnavailability> PresencesOrUnavailabilities { get; set; } = null!;
    }
}