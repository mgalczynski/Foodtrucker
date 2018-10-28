using System;
using System.Threading.Tasks;
using Persistency.Entities;
using FoodtruckOwnership = Persistency.Dtos.FoodtruckOwnership;

namespace Persistency.Services
{
    public interface IFoodtruckOwnershipService
    {
        Task CreateOwnership(Guid userId, Guid foodtruckId, Entities.OwnershipType type);
        Task<Entities.OwnershipType?> FindTypeByUserAndFoodtruck(Guid userId, Guid foodtruckId);
        Task<bool> CanManipulate(Guid userId, Guid foodtruckId, OwnershipType type);
        Task<FoodtruckOwnership> FindByUserEmailAndFoodtruck(string email, Guid foodtruckId);
        Task DeleteOwnership(string userEmail, Guid foodtruckId);
    }
}