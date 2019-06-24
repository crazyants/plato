using System.Text;
using System.Collections.Generic;
using Plato.Internal.Text.Abstractions;

namespace Plato.References.Services
{
    
    public class ReferencesTokenizer : IReferencesTokenizer
    {
        
        private const char StartChar = '#';

        // Denotes the end of a @mention
        private readonly IList<char> _terminators = new List<char>()
        {
            ',',
            ' ',
            '\r',
            '\n',
            '\t',
            '<'
        };


        private readonly IList<char> _validChars = new List<char>()
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

        public IEnumerable<IToken> Tokenize(string input)
        {

            var start = 0;
            List<Token> output = null;
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
                    if (c != StartChar && _validChars.Contains(c))
                    {
                        sb.Append(c);
                    }

                    // We've reached a terminator or the end of the input
                    if (_terminators.Contains(c) || i == input.Length - 1)
                    {
                        if (output == null)
                        {
                            output = new List<Token>();
                        }
                        output.Add(new Token()
                        {
                            Start = start,
                            End = start + sb.ToString().Length,
                            Value = sb.ToString()
                        });
                        start = 0;
                        sb = null;
                    }
                }

            }

            return output;

        }
        
    }


}
