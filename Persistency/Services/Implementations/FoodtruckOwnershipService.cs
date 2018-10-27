using System;
using System.Threading.Tasks;
using Persistency.Entities;

namespace Persistency.Services.Implementations
{
    internal class FoodtruckOwnershipService
    {
        private readonly IInternalPersistencyContext _persistencyContext;

        public FoodtruckOwnershipService(IInternalPersistencyContext persistencyContext)
        {
            _persistencyContext= persistencyContext;
        }

        public async Task CreateOwnership(Guid userId, Guid foodtruckId, Entities.OwnershipType type)
        {
            await _persistencyContext.FoodtruckOwnerships.AddAsync(new FoodtruckOwnership
                {UserId = userId, FoodtruckId = foodtruckId, Type = type});
            await _persistencyContext.SaveChangesAsync();
        }
    }
}