namespace Persistency.Dtos
{
    public sealed class CreateModifyFoodtruck
    {
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public Coordinate DefaultLocation { get; set; } = null!;
    }
}