using Persistency.Entities;

namespace Persistency.Dtos
{
    public class CreateNewOwnership
    {
        public string Email { get; set; }
        public OwnershipType Type { get; set; }
    }
}