namespace Persistency.Dtos
{
    public sealed class CheckResult
    {
        public bool IsSignedIn { get; set; }
        public User User { get; set; }
    }
}