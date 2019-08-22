using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Text.Abstractions;
using Plato.References.Models;

namespace Plato.References.Services
{

    /// <summary>
    /// Parse [#123(link text)] references into link tokens.
    /// </summary>
    public class LinkTokenizer : ILinkTokenizer
    {

        private const char StartChar = '#';
        private const char EndChar = ')';

        private const char TextStart = '(';
        private const char TextEnd= ')';

        private readonly IList<char> _validIdChars = new List<char>()
        {
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            '0'
        };

        private readonly IList<char> _invalidTextChars = new List<char>()
        {
            TextStart,
            TextEnd
        };

        /// <summary>
        /// Parse #123(title here) or #123 into tokens.
        /// We avoid RegEx intentionally and instead use simple tokens to assist with parsing.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IEnumerable<LinkToken> Tokenize(string input)
        {

            var start = 0;
            List<LinkToken> output = null;
            StringBuilder sb = null;
            
            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];

                // Start char
                if (c == StartChar)
                {
                    start = i;
                    sb = new StringBuilder();
                }

                if (sb != null)
                {

                    // Not the start character or a terminator
                    sb.Append(c);
                
                    // We've reached a terminator or the end of the input
                    if (c == EndChar || i == input.Length - 1)
                    {
                        var token = ParseToken(start, sb.ToString());
                        // The token can be null if parsing failed
                        if (token != null)
                        {
                            if (output == null)
                            {
                                output = new List<LinkToken>();
                            }
                            output.Add(token);
                        }
                        start = 0;
                        sb = null;
                    }
                }

            }

            return output;

        }

        // -----------

        LinkToken ParseToken(int start, string token)
        {

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            if (!token.StartsWith("#"))
            {
                return null;
            }
            
            
            var text = new StringBuilder();
            var value = new StringBuilder();

            var inId = false;
            var inText = false;

            foreach (var c in token)
            {

                // Keep track of our location within the token
                if (c == '#' && !inText) { inId = true; }
                if (c == TextStart) { inId = false; inText = true; }
                if (c == TextEnd) { inId = false; inText = false; }

                // Extract digits #123
                if (inId && _validIdChars.Contains(c))
                {
                    value.Append(c);
                }

                // Extract text (text)
                if (inText && !_invalidTextChars.Contains(c))
                {
                    text.Append(c);
                }

            }

            return new LinkToken()
            {
                Start = start,
                End = start + token.Length - 1,
                Text = text.ToString(),
                Value = value.ToString()
            };

        }

    }

}
