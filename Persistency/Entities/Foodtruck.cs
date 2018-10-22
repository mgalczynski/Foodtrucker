using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Persistency.Entities
{
    public class Foodtruck : BaseEntity
    {
        [Required] public string Name { get; set; }
        [Required] public string DisplayName { get; set; }
        public Point DefaultLocation { get; set; }
        public DbSet<Presence> Presences { get; set; }
    }
}