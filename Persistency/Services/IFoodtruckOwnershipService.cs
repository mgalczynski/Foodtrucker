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
        Task CreateOwnership(Guid userId, string foodtruckSlug, OwnershipType type);
        Task<OwnershipType?> FindTypeByUserAndFoodtruck(Guid userId, string foodtruckSlug);
        Task<bool> CanManipulate(Guid userId, string foodtruckSlug, OwnershipType type);
        Task<FoodtruckOwnership> FindByUserEmailAndFoodtruck(string email, string foodtruckSlug);
        Task DeleteOwnership(string userEmail, string foodtruckSlug);
        Task<IList<FoodtruckOwnership>> FindFoodtruckOwnershipsByFoodtruck(string foodtruckSlug);
        Task<IList<FoodtruckWithOwnership>> FindFoodtruckOwnershipsByUser(Guid userId);
        Task ChangeOwnership(string foodtruckSlug, string userEmail, OwnershipType type);
    }
}