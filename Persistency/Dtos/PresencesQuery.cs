using System;
using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class PresencesQuery
    {
        public ISet<Guid> Ids { get; set; }
    }
}