using System.Collections.Generic;
using System.Text;

namespace Plato.Mentions.Services
{

 
    public class MentionsTokenizer : IMentionsTokenizer
    {
        
        private const char StartChar = '@';

        // Denotes the end of a @mention
        private readonly IList<char> _terminators = new List<char>()
        {
            ',',
            ' ',
            '\r',
            '\n'
        };

        public IList<MentionToken> Tokenize(string input)
        {

            var start = 0;
            List<MentionToken> output = null;
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
                    if (c != StartChar && !_terminators.Contains(c))
                    {
                        sb.Append(c);
                    }

                    // We've reached a terminator or the end of the input
                    if (_terminators.Contains(c) || i == input.Length - 1)
                    {
                        if (output == null)
                        {
                            output = new List<MentionToken>();
                        }
                        output.Add(new MentionToken()
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

    public class MentionToken
    {

        public int Start { get; set; }

        public int End { get; set; }

        public string Value { get; set; }

    }

}
