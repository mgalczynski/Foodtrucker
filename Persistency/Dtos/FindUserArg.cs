using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class FindUserArg
    {
        public string Query { get; set; } = null!;
		public IList<string> Except { get; set; } = new List<string>();
	}
}