namespace Persistency.Dtos
{
    public sealed class CreateModifyFoodtruck
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Coordinate DefaultLocation { get; set; }
    }
}