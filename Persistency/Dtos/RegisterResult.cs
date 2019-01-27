using System.Collections.Generic;

namespace Persistency.Dtos
{
    public class RegisterResult
    {
        public IList<string> Errors;
        public bool Successful { get; set; }
        public User User { get; set; }
    }
}