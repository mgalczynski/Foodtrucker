using System;

namespace Persistency.Services
{
    public class ValidationException<TDto>: Exception
    {
        internal ValidationException(TDto dto)
        {
            Dto = dto;
        }

        public TDto Dto { get; }
    }
}