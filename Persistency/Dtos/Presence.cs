﻿using System;

namespace Persistency.Dtos
{
    public sealed class PresenceOrUnavailability
    {
        public Guid Id { get; set; }
        public string FoodtruckSlug { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Coordinate Location { get; set; }
    }
}