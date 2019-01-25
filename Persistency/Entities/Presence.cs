using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Persistency.Entities
{
    public class Presence : BaseEntity
    {
        [Required] public Guid FoodtruckId { get; set; }
        public Foodtruck Foodtruck { get; set; }
        [Required] public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [Required] public string Title { get; set; }
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "geography(Point,4326)")]
        public Point Location { get; set; }
    }
}