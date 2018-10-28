using System;
using System.Threading.Tasks;

namespace Persistency.Services
{
    public interface IFoodtruckOwnershipService
    {
        Task CreateOwnership(Guid userId, Guid foodtruckId, Entities.OwnershipType type);
    }
}