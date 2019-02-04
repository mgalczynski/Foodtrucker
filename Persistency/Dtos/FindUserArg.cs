using System;
using System.Collections.Generic;

namespace Persistency.Dtos
{
    public sealed class FindUserArg
    {
        public string Query { get; set; }
        public IList<string> Except { get; set; }
    }
}