using System;

namespace Persistency.Dtos
{
    public sealed class CreateModifyPresenceOrUnavailability
    {
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Coordinate Location { get; set; } = null!;
    }
}