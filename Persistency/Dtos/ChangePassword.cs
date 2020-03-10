namespace Persistency.Dtos
{
    public sealed class ChangePassword
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}