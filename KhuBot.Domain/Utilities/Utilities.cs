using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.Utilities
{
    public static class Utilities
    {
        public static string? FirstCharToLower(this string? input) =>
            input switch
            {
                null or "" => null,
                _ => string.Concat(input[0].ToString().ToLower(), input.AsSpan(1))
            };
    }
}
