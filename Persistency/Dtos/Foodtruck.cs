using System;

namespace Persistency.Dtos
{
    public sealed class Foodtruck
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Coordinate DefaultLocation { get; set; }
    }
}