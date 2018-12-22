using Plato.Internal.Drawing.Abstractions;

namespace Plato.Internal.Drawing.Captcha
{

    public class CaptchaOptions
    {

    }

    public class InMemoryCaptchaRenderer
    {

        private readonly IDisposableBitmap _renderer;

        public InMemoryCaptchaRenderer(
            IDisposableBitmap renderer)
        {
            _renderer = renderer;
        }


    }
}
