using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class GenericListResult<T>
    {
        public IList<T> Result { get; set; }
    }
}