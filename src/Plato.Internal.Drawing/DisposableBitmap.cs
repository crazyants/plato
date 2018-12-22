using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Plato.Internal.Drawing
{
    
    public class DisposableBitmap : IDisposableBitmap
    {
        private Bitmap _butmap;

        private readonly BitmapOptions _options;

        public DisposableBitmap()
        {
            _options = new BitmapOptions();
        }

        public IDisposableBitmap Configure(Action<BitmapOptions> action)
        {
            action(_options);
            return this;
        }
        
        public Bitmap Render(Action<Bitmap> renderer)
        {
            _butmap = new Bitmap(_options.Width, _options.Height, PixelFormat.Format32bppArgb);
            renderer(_butmap);
            return _butmap;
        }

        public void Dispose()
        {
            _butmap?.Dispose();
        }

    }

}
