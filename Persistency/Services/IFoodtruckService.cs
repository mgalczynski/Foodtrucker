using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistency.Dtos;

namespace Persistency.Services
{
    public interface IFoodtruckService : IBaseService<Foodtruck>
    {
        Task<IList<Foodtruck>> FindFoodTrucksWithin(Coordinate coordinate, double distance);
        Task<Foodtruck> CreateNewFoodtruck(CreateNewFoodtruck createNewFoodtruck);
        Task MarkAsDeleted(string slug);
        Task<IList<Foodtruck>> FindFoodTrucksWithin(Coordinate topLeft, Coordinate bottomRight);
        Task<Foodtruck> FindBySlug(string slug);
        Task<IList<Foodtruck>> FindBySlugs(IEnumerable<string> slugs);
        Task<FoodtruckDetailed> FindBySlugDetailed(string slug);
    }

    internal interface IInternalFoodtruckService : IFoodtruckService
    {
        Task<Guid> FindFoodtruckIdBySlug(string slug);
    }
}