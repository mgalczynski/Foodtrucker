using System;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace Persistency.Entities
{
    internal class Foodtruck
    {
        public Guid Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string DisplayName { get; set; }
        public Point DefaultLocation { get; set; }
    }
}