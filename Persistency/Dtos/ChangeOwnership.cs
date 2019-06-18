using OwnershipType = Persistency.Entities.OwnershipType;

namespace Persistency.Dtos
{
    public sealed class ChangeOwnership
    {
        public string Email { get; set; }
        public OwnershipType Type { get; set; }
    }
}