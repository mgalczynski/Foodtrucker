using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class RegisterResult
    {
		public IList<string> Errors { get; set; } = new List<string>();
		public bool Successful { get; set; }
		public IList<string> Roles { get; set; } = new List<string>();
		public User User { get; set; } = null!;
    }
}