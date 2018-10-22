using System;
using System.Collections.Generic;
using System.Text;

namespace Persistency.Entities
{
    internal abstract class BaseEntity
    {
        public Guid Id { get; set; }
    }
}