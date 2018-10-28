using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Persistency.Entities;

namespace Persistency.Services.Implementations
{
    internal class FoodtruckOwnershipService : IFoodtruckOwnershipService
    {
        private readonly IInternalPersistencyContext _persistencyContext;

        public FoodtruckOwnershipService(IInternalPersistencyContext persistencyContext)
        {
            _persistencyContext = persistencyContext;
        }

        public async Task CreateOwnership(Guid userId, Guid foodtruckId, Entities.OwnershipType type)
        {
            await _persistencyContext.FoodtruckOwnerships.AddAsync(new FoodtruckOwnership
                {UserId = userId, FoodtruckId = foodtruckId, Type = type});
            await _persistencyContext.SaveChangesAsync();
        }

        private async Task<FoodtruckOwnership> FindByUserAndFoodtruck(Guid userId, Guid foodtruckId) =>
            await _persistencyContext.FoodtruckOwnerships
                .FirstOrDefaultAsync(e => e.UserId == userId && e.FoodtruckId == foodtruckId);

        public async Task<OwnershipType?> FindTypeByUserAndFoodtruck(Guid userId, Guid foodtruckId) =>
            (await FindByUserAndFoodtruck(userId, foodtruckId))?.Type;
    }
}