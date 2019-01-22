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
        internal static Point ToDbPoint(this Coordinate coordinate) =>
            coordinate == null ? null : CreatePointWithSrid(coordinate.Latitude, coordinate.Longitude);

        internal static Coordinate ToCoordinate(this Point point) =>
            point == null ? null : new Coordinate {Latitude = point.Y, Longitude = point.X};

        internal static Point CreatePointWithSrid(double latitude, double longitude) =>
            new Point(longitude, latitude) {SRID = 4326};

        internal static async Task<IList<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable) =>
            await queryable.ProjectTo<TDestination>().ToListAsync();


        internal static async Task<TDestination> MapAsync<TDestination, TSource>(this Task<TSource> task)
            where TDestination : class
        {
            var obj = await task;
            return obj == null ? null : Mapper.Map<TDestination>(obj);
        }
    }
}