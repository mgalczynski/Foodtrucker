using System;

namespace Persistency.Dtos
{
    public sealed class FoodtrucksQuery
    {
        public Coordinate TopLeft { get; set; } = null!;
        public Coordinate BottomRight { get; set; } = null!;
        public DateTime StartEndTime { get; set; }
    }
}