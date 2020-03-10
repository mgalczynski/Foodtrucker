using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class LoginResult
    {
        public bool Successful { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsNotAllowed { get; set; }
		public IList<string> Roles { get; set; } = new List<string>();
		public User User { get; set; } = null!;
    }
}