using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NetTopologySuite.Geometries;

namespace Persistency.Entities
{
    public class Presence : BaseEntity
    {
        [Required] public Guid FoodTruckId { get; set; }
        public Foodtruck Foodtruck { get; set; }
        [Required] public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [Required] public string Title { get; set; }
        public string Description { get; set; }
        [Required] public Point Location { get; set; }
    }
}