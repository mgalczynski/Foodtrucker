using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class FoodtrucksWithinResult
    {
        public IList<Foodtruck> Foodtrucks { get; set; }
        public IList<PresenceOrUnavailability> PresencesOrUnavailabilities { get; set; }
    }
}