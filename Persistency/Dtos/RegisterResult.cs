using System.Collections.Generic;

namespace WebApplication.Dtos
{
    public class RegisterResult
    {
        public bool Successful { get; set; }
        public IList<string> Errors;
    }
}