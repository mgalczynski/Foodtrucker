namespace Persistency.Dtos
{
    public sealed class Foodtruck
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Coordinate DefaultLocation { get; set; }
    }
}