using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistency.Dtos;
using Persistency.Entities;
using FoodtruckOwnership = Persistency.Dtos.FoodtruckOwnership;

namespace Persistency.Services
{
    public interface IFoodtruckOwnershipService
    {
        Task CreateOwnership(Guid userId, Guid foodtruckId, OwnershipType type);
        Task<OwnershipType?> FindTypeByUserAndFoodtruck(Guid userId, Guid foodtruckId);
        Task<bool> CanManipulate(Guid userId, Guid foodtruckId, OwnershipType type);
        Task<FoodtruckOwnership> FindByUserEmailAndFoodtruck(string email, Guid foodtruckId);
        Task DeleteOwnership(string userEmail, Guid foodtruckId);
        Task<IList<FoodtruckOwnership>> FindFoodtruckOwnershipsByFoodtruck(Guid foodtruckId);
        Task<IList<FoodtruckWithOwnership>> FindFoodtruckOwnershipsByUser(Guid userId);
        Task ChangeOwnership(Guid foodtruckId, string userEmail, OwnershipType type);
    }
}