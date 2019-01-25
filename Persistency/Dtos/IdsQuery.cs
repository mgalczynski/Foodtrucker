using System;
using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class IdsQuery
    {
        public IList<Guid> Ids { get; set; }
    }
}