using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Text.Extensions
{
    public static class TokenExtensions
    {

        public static bool IsSurroundedBy(
            this IToken token,
            string input,
            char startChar,
            char endChar)
        {

            var startIndex = token.Start > 0 ? token.Start - 1 : token.Start;
            var endIndex = token.End < input.Length ? token.End + 1 : token.End;

            var startCharToCompare1 = input[token.Start];

            var startCharToCompare = input[startIndex];
            var endCharToCompare = input[endIndex];

            var start = startCharToCompare == startChar;
            var end = endCharToCompare == endChar;

            return start && end;
        }
    }
}
