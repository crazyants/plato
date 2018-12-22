using System;
using System.IO;

namespace Plato.Internal.Drawing.Abstractions.Letters
{
    public interface ILetterRenderer : IDisposable
    {
        Stream GetLetter(LetterOptions options);
    }

}
