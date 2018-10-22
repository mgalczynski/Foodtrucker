namespace WebApplication.Dtos
{
    public class LoginUser:RegisterUser
    {
        public bool RememberMe { get; set; }
    }
}