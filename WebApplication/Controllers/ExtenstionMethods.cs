using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Persistency.Dtos;

namespace WebApplication.Controllers
{
    internal static class ExtenstionMethods
    {
        internal static ActionResult<GenericListResult<T>> ToResult<T>(this IEnumerable<T> list)
        {
            return new OkObjectResult(new GenericListResult<T> {Result = new List<T>(list)});
        }
    }
}