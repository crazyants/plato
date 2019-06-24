using System.Collections.Generic;

namespace Plato.Internal.Text.Abstractions
{
    public interface ITokenizer
    {

        IEnumerable<IToken> Tokenize(string input);

    }

    public class Token : IToken
    {

        public int Start { get; set; }

        public int End { get; set; }

        public string Value { get; set; }

    }

    public interface IToken
    {
        int Start { get; set; }

        int End { get; set; }

        string Value { get; set; }

    }

}
