using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Persistency.Services;
using Persistency.Services.Implementations;

namespace Persistency
{
    public static class Persistency
    {
        public static void RegisterPersistency(IServiceCollection collection)
        {
            collection.AddDbContext<PersistencyContext>();
            collection.AddScoped<IFoodtruckService, FoodtruckService>();
            collection.AddScoped<IPresenceService, PresenceService>();
            Mapper.Initialize(InitializeMapper);
        }

        private static void InitializeMapper(IMapperConfigurationExpression mapper)
        {
            mapper.CreateMap<Dtos.Foodtruck, Entities.Foodtruck>().ReverseMap();
            mapper.CreateMap<Dtos.Presence, Entities.Presence>().ReverseMap();
            mapper.CreateMap<Dtos.Coordinate, Point>().ConvertUsing(ExtensionMethods.ToDbPoint);
            mapper.CreateMap<Point, Dtos.Coordinate>().ConvertUsing(ExtensionMethods.ToCoordinate);
        }
    }
}