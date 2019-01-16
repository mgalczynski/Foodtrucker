namespace Persistency.Dtos
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsNotAllowed { get; set; }
        public User User { get; set; }
    }
}