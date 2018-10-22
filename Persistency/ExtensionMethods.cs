using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Persistency.Dtos;

namespace Persistency
{
    internal static class ExtensionMethods
    {
        internal static Point ToDbPoint(this Coordinate coordinate) =>
            CreatePointWithSrid(coordinate.Latitude, coordinate.Longitude);

        internal static Coordinate ToCoordinate(this Point point) =>
            new Coordinate {Latitude = point.X, Longitude = point.Y};

        internal static Point CreatePointWithSrid(double latitude, double longitude) =>
            new Point(latitude, longitude) {SRID = 4326};

        internal static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable iQueryable) =>
            iQueryable.ProjectTo<TDestination>().ToListAsync();
    }
}