using System.Collections.Generic;
using System.Text;
using Plato.References.Models;

namespace Plato.References.Services
{

    /// <summary>
    /// Simple tokenizer to convert #123(title here) or #123 string values into tokens.
    /// We avoid RegEx intentionally and instead use tokens to represent what we need to parse. 
    /// </summary>
    public class LinkTokenizer : ILinkTokenizer
    {

        /// #123(text)
        /// ^ StartChar
        /// #123(text)
        ///  ^^^ Value
        /// #123(text)
        ///     ^ TextStart
        /// #123(text)
        ///      ^^^^ Text
        /// #123(text)
        ///          ^ TextEnd

        private const char StartChar = '#';
        private const char TextStart = '(';
        private const char TextEnd= ')';

        /// <summary>
        /// The value can only be a number. I.e. an entity Id.
        /// </summary>
        private readonly IList<char> _validValueChars = new List<char>()
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

        /// <summary>
        /// Ensure text delimiters don't appear within text value. 
        /// </summary>
        private readonly IList<char> _invalidTextChars = new List<char>()
        {
            TextStart,
            TextEnd
        };

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

                // We are within a token
                if (sb != null)
                {
               
                    // Add tokens contents
                    sb.Append(c);
                
                    // We've reached the end character or the end of the input
                    if (c == TextEnd || i == input.Length - 1)
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

                        // Exit out of token
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

            if (!token.StartsWith(StartChar.ToString()))
            {
                return null;
            }
            
            // Holds out text & value
            var text = new StringBuilder();
            var value = new StringBuilder();

            // Flags to indicate we are inside of text or value
            var inValue = false;
            var inText = false;

            foreach (var c in token)
            {

                // Keep track of our location within the token
                if (c == StartChar && !inText) { inValue = true; }
                if (c == TextStart) { inValue = false; inText = true; }
                if (c == TextEnd) { inValue = false; inText = false; }

                // Extract digits #123
                if (inValue && _validValueChars.Contains(c))
                {
                    value.Append(c);
                }

                // Extract text (text)
                if (inText && !_invalidTextChars.Contains(c))
                {
                    text.Append(c);
                }

            }

            // Return an object representing our found token
            // We'll use this within the references parser 
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
