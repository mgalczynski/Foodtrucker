using OwnershipType = Persistency.Entities.OwnershipType;

namespace Persistency.Dtos
{
    public class ChangeOwnership
    {
        public string Email { get; set; }
        public OwnershipType Type { get; set; }
    }
}