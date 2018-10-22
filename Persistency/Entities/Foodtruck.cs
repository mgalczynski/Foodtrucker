using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace Persistency.Entities
{
    public class Foodtruck : BaseEntity
    {
        [Required] public string Name { get; set; }
        [Required] public string DisplayName { get; set; }
        public Point DefaultLocation { get; set; }
        public IList<Presence> Presences { get; set; }
    }
}