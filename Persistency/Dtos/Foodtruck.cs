namespace Persistency.Dtos
{
    public sealed class Foodtruck
    {
        public string Slug { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public Coordinate DefaultLocation { get; set; } = null!;
    }
}