namespace Persistency.Dtos
{
    public class InsertStatus<TId>
    {
        public TId Id { get; set; }
        public bool Successful { get; set; }
        public string Description { get; set; }
    }
}