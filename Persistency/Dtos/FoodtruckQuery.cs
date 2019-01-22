namespace Persistency.Dtos
{
    public sealed class FoodtruckQuery
    {
        public Coordinate TopLeft { get; set; }
        public Coordinate BottomRight { get; set; }
    }
}