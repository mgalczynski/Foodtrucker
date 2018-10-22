using System;
using System.Collections.Generic;
using System.Text;

namespace Persistency.Dtos
{
    public class Foodtruck
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Coordinate DefaultLocation { get; set; }
    }
}