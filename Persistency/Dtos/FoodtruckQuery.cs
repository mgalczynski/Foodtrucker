using System;

namespace Persistency.Dtos
{
    public sealed class FoodtrucksQuery
    {
        public Coordinate TopLeft { get; set; }
        public Coordinate BottomRight { get; set; }
        public DateTime StartEndTime { get; set; }
    }
}