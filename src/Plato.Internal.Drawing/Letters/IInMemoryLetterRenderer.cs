using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Plato.Internal.Drawing.Letters
{

    public interface ILetterRenderer : IDisposable
    {
        Stream GetLetter(LetterOptions options);
    }

    public interface IInMemoryLetterRenderer : ILetterRenderer
    {
  
    }
}
