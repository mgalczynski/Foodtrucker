namespace Persistency.Dtos
{
    public sealed class ResponseWithStatus<TDto>
    {
        public TDto Dto { get; set; }
        public bool Successful { get; set; }
        public string Description { get; set; }
    }
}