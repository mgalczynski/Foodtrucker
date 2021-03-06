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

        private readonly IInternalFoodtruckService _foodtruckService;
        private readonly IInternalPersistencyContext _persistencyContext;
        private readonly IRuntimeMapper _runtimeMapper;

        public FoodtruckOwnershipService(IInternalPersistencyContext persistencyContext, IRuntimeMapper runtimeMapper, IInternalFoodtruckService foodtruckService)
        {
            _persistencyContext = persistencyContext;
            _foodtruckService = foodtruckService;
            _runtimeMapper = runtimeMapper;
        }

        private DbSet<Entity> DbSet =>
            _persistencyContext.FoodtruckOwnerships;

        private IQueryable<Entity> Queryable => DbSet
            .Where(e => !e.Foodtruck.Deleted);

        public async Task CreateOwnership(Guid userId, string foodtruckSlug, OwnershipType type)
        {
            await DbSet.AddAsync(new Entity {UserId = userId, FoodtruckId = await _foodtruckService.FindFoodtruckIdBySlug(foodtruckSlug), Type = type});
            await _persistencyContext.SaveChangesAsync();
        }

        public async Task<FoodtruckOwnership> FindByUserEmailAndFoodtruck(string email, string foodtruckSlug) =>
            _runtimeMapper.Map<FoodtruckOwnership>(await FindEntityByUserEmailAndFoodtruck(email, foodtruckSlug));


        public async Task<OwnershipType?> FindTypeByUserAndFoodtruck(Guid userId, string foodtruckSlug) =>
            (await FindEntityByUserAndFoodtruck(userId, foodtruckSlug))?.Type;

        public async Task<OwnershipType?> FindTypeByUserAndPresenceOrUnavailability(Guid userId, Guid presenceId) =>
            (await FindEntityByUserAndPresenceOrUnavailability(userId, presenceId))?.Type;

        public async Task<bool> CanManipulate(Guid userId, string foodtruckSlug, OwnershipType type) =>
            _revesedAccessDict[type].Contains((await FindEntityByUserAndFoodtruck(userId, foodtruckSlug)).Type);

        public async Task<IList<FoodtruckOwnership>> FindFoodtruckOwnershipsByFoodtruck(string foodtruckSlug) =>
            await Queryable
                .Where(e => e.Foodtruck.Slug == foodtruckSlug)
                .ProjectToListAsync<FoodtruckOwnership>(_runtimeMapper.ConfigurationProvider);

        public async Task<IList<FoodtruckWithOwnership>> FindFoodtruckOwnershipsByUser(Guid userId) =>
            await Queryable
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.Foodtruck.Name)
                .ProjectToListAsync<FoodtruckWithOwnership>(_runtimeMapper.ConfigurationProvider);

        public async Task ChangeOwnership(string foodtruckSlug, string userEmail, OwnershipType type)
        {
            (await Queryable.FirstOrDefaultAsync(o => o.Foodtruck.Slug == foodtruckSlug && o.User.Email == userEmail)).Type = type;
            await _persistencyContext.SaveChangesAsync();
        }

        public async Task DeleteOwnership(string userEmail, string foodtruckSlug)
        {
            DbSet.Remove(await FindEntityByUserEmailAndFoodtruck(userEmail, foodtruckSlug));
            await _persistencyContext.SaveChangesAsync();
        }

        private async Task<Entity> FindEntityByUserAndFoodtruck(Guid userId, string foodtruckSlug) =>
            await Queryable.FirstOrDefaultAsync(e => e.UserId == userId && e.Foodtruck.Slug == foodtruckSlug);

        private async Task<Entity> FindEntityByUserEmailAndFoodtruck(string email, string foodtruckSlug) =>
            await Queryable.FirstOrDefaultAsync(e => e.User.Email == email && e.Foodtruck.Slug == foodtruckSlug);

        private async Task<Entity> FindEntityByUserAndPresenceOrUnavailability(Guid userId, Guid presenceId) =>
            await Queryable.FirstOrDefaultAsync(e => e.UserId == userId && e.Foodtruck.PresencesOrUnavailabilities.Any(p => p.Id == presenceId));
    }
}