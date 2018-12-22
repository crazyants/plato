using System;
using System.Drawing;

namespace Plato.Internal.Drawing
{
    public interface IDisposableBitmap : IDisposable
    {

        IDisposableBitmap Configure(Action<BitmapOptions> action);

        Bitmap Render(Action<Bitmap> renderer);

    }

}
