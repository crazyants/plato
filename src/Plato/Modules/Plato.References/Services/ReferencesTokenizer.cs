using System.Collections.Generic;
using System.Text;
using Plato.Internal.Text.Abstractions;

namespace Plato.References.Services
{
    
    public class ReferencesTokenizer : IReferencesTokenizer
    {
        
        private const char StartChar = '#';


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
            List<ReferenceToken> output = null;
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
                    if (_validChars.Contains(c))
                    {
                        sb.Append(c);
                    }

                    // We've reached a terminator or the end of the input
                    if (!_validChars.Contains(c) || i == input.Length - 1)
                    {
                        if (output == null)
                        {
                            output = new List<ReferenceToken>();
                        }
                        output.Add(new ReferenceToken()
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

    public class ReferenceToken : IToken
    {

        public int Start { get; set; }

        public int End { get; set; }

        public string Value { get; set; }

    }

}
