using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Persistency.Dtos;

namespace Persistency
{
    internal static class ExtensionMethods
    {
        internal static Point ToDbPoint(this Dtos.Coordinate coordinate) =>
            CreatePointWithSrid(coordinate.Latitude, coordinate.Longitude);

        internal static Dtos.Coordinate ToCoordinate(this Point point) =>
            new Dtos.Coordinate {Latitude = point.Y, Longitude = point.X};

        internal static Point CreatePointWithSrid(double latitude, double longitude) =>
            new Point(longitude, latitude) {SRID = 4326};

        internal static async Task<IList<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration) =>
            await queryable.ProjectTo<TDestination>(configuration).ToListAsync();


        internal static async Task<TDestination> MapAsync<TDestination, TSource>(this Task<TSource> task, IRuntimeMapper runtimeMapper)
            where TDestination : class =>
            runtimeMapper.Map<TDestination>(await task);
    }
}