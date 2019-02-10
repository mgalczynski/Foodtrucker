using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Persistency.Entities
{
    public class Foodtruck : BaseEntity
    {
        [Required] public string Name { get; set; }
        [Required] public string DisplayName { get; set; }
        [Required] public string Slug { get; set; }

        [Column(TypeName = "geography(Point,4326)")]
        public Point DefaultLocation { get; set; }

        public IList<Presence> Presences { get; set; }

        public IList<FoodtruckOwnership> Ownerships { get; set; }
        public bool Deleted { get; set; }
    }
}