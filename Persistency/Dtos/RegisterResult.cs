using System.Collections.Generic;

namespace Persistency.Dtos
{
    public class RegisterResult
    {
        public bool Successful { get; set; }
        public User User { get; set; }
        public IList<string> Errors;
    }
}