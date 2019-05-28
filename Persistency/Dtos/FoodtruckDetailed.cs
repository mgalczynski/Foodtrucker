using System;
using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class FoodtruckDetailed
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Coordinate DefaultLocation { get; set; }
        public IList<PresenceOrUnavailability> PresencesOrUnavailabilities { get; set; }
        public IList<Ownership> Ownerships { get; set; }
    }
}