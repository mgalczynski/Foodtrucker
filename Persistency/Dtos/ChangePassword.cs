namespace Persistency.Dtos
{
    public sealed class ChangePassword
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}