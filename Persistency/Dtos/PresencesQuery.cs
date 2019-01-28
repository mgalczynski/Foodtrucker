using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class PresencesQuery
    {
        public ISet<string> Slugs { get; set; }
    }
}