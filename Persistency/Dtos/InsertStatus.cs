namespace Persistency.Dtos
{
    public class InsertStatus<TId>
    {
        public TId Id { get; set; }
        public bool Successful { get; set; }
        public bool Description { get; set; }
    }
}