using System.Collections.Generic;

namespace Plato.Internal.Text.Abstractions
{
    public interface ITokenizer
    {

        IEnumerable<IToken> Tokenize(string input);

    }

    public interface IToken
    {
        int Start { get; set; }

        int End { get; set; }

        string Value { get; set; }

    }

}
