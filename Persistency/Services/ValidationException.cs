using System;

namespace Persistency.Services
{
    public class ValidationException<TDto> : Exception
    {
        internal ValidationException(TDto dto) : base("Conflicts with:")
        {
            Dto = dto;
        }

        internal ValidationException(string message) : base(message)
        {
        }

        public TDto Dto { get; }
    }
}