using OwnershipType = Persistency.Entities.OwnershipType;

namespace Persistency.Dtos
{
    public sealed class CreateNewOwnership
    {
        public string Email { get; set; } = null!;
        public OwnershipType Type { get; set; }
    }
}