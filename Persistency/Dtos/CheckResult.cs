using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class CheckResult
    {
        public bool IsSignedIn { get; set; }
        public IList<string> Roles { get; set; }
        public User User { get; set; }
    }
}