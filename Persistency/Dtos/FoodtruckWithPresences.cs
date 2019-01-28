using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class FoodtruckWithPresences
    {
        public Foodtruck Foodtruck { get; set; }
        public IList<Presence> Presences { get; set; }
    }
}