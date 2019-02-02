using OwnershipType = Persistency.Entities.OwnershipType;

namespace Persistency.Dtos
{
    public sealed class Ownership
    {
        public User User { get; set; }
        public OwnershipType Type { get; set; }
    }
}