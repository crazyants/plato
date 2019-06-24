using System.Collections.Generic;

namespace Plato.Mentions.Services
{
    public interface IMentionsTokenizer
    {
        IList<MentionToken> Tokenize(string input);

    }

}
