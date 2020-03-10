using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class PresencesOrUnavailabilitiesQuery
    {
		public ISet<string> Slugs { get; set; } = new HashSet<string>();
	}
}