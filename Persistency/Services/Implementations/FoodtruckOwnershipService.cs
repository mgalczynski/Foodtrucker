using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Persistency.Dtos;
using Persistency.Entities;
using Entity = Persistency.Entities.FoodtruckOwnership;
using FoodtruckOwnership = Persistency.Dtos.FoodtruckOwnership;

namespace Persistency.Services.Implementations
{
    internal class FoodtruckOwnershipService : IFoodtruckOwnershipService
    {
        private static readonly IDictionary<OwnershipType, IList<OwnershipType>> _accessDict =
            new Dictionary<OwnershipType, IList<OwnershipType>>
            {
                {
                    OwnershipType.OWNER,
                    new List<OwnershipType> {OwnershipType.OWNER, OwnershipType.ADMIN, OwnershipType.REPORTER}
                },
                {OwnershipType.ADMIN, new List<OwnershipType> {OwnershipType.REPORTER}},
                {OwnershipType.REPORTER, new List<OwnershipType>()}
            };

        private static readonly IDictionary<OwnershipType, IList<OwnershipType>> _revesedAccessDict =
            _accessDict.Aggregate(new Dictionary<OwnershipType, IList<OwnershipType>>(),
                (acc, pair) =>
                {
                    foreach (var ownershipType in pair.Value)
                    {
                        IList<OwnershipType> list = null;
                        if (!acc.TryGetValue(ownershipType, out list)) list = acc[ownershipType] = new List<OwnershipType>();

                        list.Add(pair.Key);
                    }

                    return acc;
                });

        private readonly IInternalPersistencyContext _persistencyContext;

        public FoodtruckOwnershipService(IInternalPersistencyContext persistencyContext)
        {
            _persistencyContext = persistencyContext;
        }

        public async Task CreateOwnership(Guid userId, Guid foodtruckId, OwnershipType type)
        {
            await _persistencyContext.FoodtruckOwnerships.AddAsync(new Entity
                {UserId = userId, FoodtruckId = foodtruckId, Type = type});
            await _persistencyContext.SaveChangesAsync();
        }

        public async Task<FoodtruckOwnership> FindByUserEmailAndFoodtruck(string email, Guid foodtruckId) =>
            Mapper.Map<FoodtruckOwnership>(await FindEntityByUserEmailAndFoodtruck(email, foodtruckId));


        public async Task<OwnershipType?> FindTypeByUserAndFoodtruck(Guid userId, Guid foodtruckId) =>
            (await FindEntityByUserAndFoodtruck(userId, foodtruckId))?.Type;

        public async Task<bool> CanManipulate(Guid userId, Guid foodtruckId, OwnershipType type) =>
            _revesedAccessDict[type].Contains((await FindEntityByUserAndFoodtruck(userId, foodtruckId)).Type);

        public async Task<IList<FoodtruckOwnership>> FindFoodtruckOwnershipsByFoodtruck(Guid foodtruckId) =>
            await _persistencyContext.FoodtruckOwnerships
                .Where(e => e.FoodtruckId == foodtruckId)
                .ProjectToListAsync<FoodtruckOwnership>();

        public async Task<IList<FoodtruckWithOwnership>> FindFoodtruckOwnershipsByUser(Guid userId) =>
            await _persistencyContext.FoodtruckOwnerships
                .Where(e => e.UserId == userId)
                .ProjectToListAsync<FoodtruckWithOwnership>();

        public async Task DeleteOwnership(string userEmail, Guid foodtruckId)
        {
            _persistencyContext.FoodtruckOwnerships.Remove(
                await FindEntityByUserEmailAndFoodtruck(userEmail, foodtruckId));
            await _persistencyContext.SaveChangesAsync();
        }

        private async Task<Entity> FindEntityByUserAndFoodtruck(Guid userId, Guid foodtruckId) =>
            await _persistencyContext.FoodtruckOwnerships
                .FirstOrDefaultAsync(e => e.UserId == userId && e.FoodtruckId == foodtruckId);

        private async Task<Entity> FindEntityByUserEmailAndFoodtruck(string email, Guid foodtruckId) =>
            await _persistencyContext.FoodtruckOwnerships
                .FirstOrDefaultAsync(e => e.User.Email == email && e.FoodtruckId == foodtruckId);
    }
}