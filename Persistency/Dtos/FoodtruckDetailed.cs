using System;
using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class FoodtruckDetailed
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public Coordinate DefaultLocation { get; set; } = null!;
        public IList<PresenceOrUnavailability> PresencesOrUnavailabilities { get; set; } = null!;
        public IList<Ownership> Ownerships { get; set; } = null!;
    }
}