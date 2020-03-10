using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Persistency.Entities
{
    public class Foodtruck : BaseEntity
    {
		[Required] public string Name { get; set; } = null!;
		[Required] public string DisplayName { get; set; } = null!;
        [Required] public string Slug { get; set; } = null!;

        [Column(TypeName = "geography(Point,4326)")]
        public Point DefaultLocation { get; set; } = null!;

		public IList<PresenceOrUnavailability> PresencesOrUnavailabilities { get; set; } = new List<PresenceOrUnavailability>();

		public IList<FoodtruckOwnership> Ownerships { get; set; } = new List<FoodtruckOwnership>();
		public bool Deleted { get; set; }
    }
}