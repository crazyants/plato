using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Markdown.Services
{
    public interface IMarkdownParserFactory
    {
        IMarkdownParser GetParser();
    }

}
