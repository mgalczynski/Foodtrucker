namespace WebApplication.Dtos
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsNotAllowed { get; set; }
    }
}