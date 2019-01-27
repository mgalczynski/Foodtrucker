namespace Persistency.Dtos
{
    public sealed class CreateNewFoodtruck
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Coordinate DefaultLocation { get; set; }
    }
}