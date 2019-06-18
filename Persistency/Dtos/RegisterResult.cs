using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class RegisterResult
    {
        public IList<string> Errors;
        public bool Successful { get; set; }
        public User User { get; set; }
    }
}