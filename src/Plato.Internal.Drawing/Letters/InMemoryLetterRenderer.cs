using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Plato.Internal.Drawing.Extensions;
using Plato.Internal.Drawing.Abstractions;
using Plato.Internal.Drawing.Abstractions.Letters;

namespace Plato.Internal.Drawing.Letters
{
    
    public class InMemoryLetterRenderer : IInMemoryLetterRenderer
    {

        private MemoryStream _stream;
     
        private readonly IDisposableBitmap _renderer;

        public InMemoryLetterRenderer(IDisposableBitmap renderer)
        {
            _renderer = renderer;
        }

        public Stream GetLetter(LetterOptions options)
        {

            _stream = new MemoryStream();
            using (var renderer = _renderer)
            {

                var result = renderer.Configure(o =>
                    {
                        o.Width = options.Width;
                        o.Height = options.Height;
                    })
                    .Render(bitmap =>
                    {

                        // Create a graphics object for drawing.
                        var g = Graphics.FromImage(bitmap);
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        var rect = new RectangleF(0, 0, options.Width, options.Height);

                        // Fill in the background.

                        var backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                            rect,
                            options.BackColor.ToColor(Color.CornflowerBlue),
                            options.BackColor.ToColor(Color.CornflowerBlue),
                            System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                        g.FillRectangle(backBrush, rect);
                 
                        // create solid dark brush
                        var gradBrushDark = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.Black,
                            Color.Gray,
                            System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                        
                        // Set up the text font.
                        SizeF size;
                        float fontSize = 200;
                        Font font = null;
                        // Adjust the font size until the text fits within the image.
                        do
                        {
                            fontSize -= 1;
                            font = new Font(options.FontFamily, fontSize, FontStyle.Bold);
                            size = g.MeasureString(options.Letter.ToString(), font);
                        } while (size.Width > rect.Width);

                        // Set up the text format.
                        var format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        
                        // Create a path using the text and warp it randomly - nice :)
                        var path = new System.Drawing.Drawing2D.GraphicsPath();
                        path.AddString(options.Letter.ToString(),
                            font.FontFamily,
                            System.Convert.ToInt32(font.Style),
                            font.Size,
                            rect,
                            format);
                        float v = 8f;
                        PointF[] points = {
                            new PointF((Int32)rect.Width / v, (Int32)rect.Height / v),
                            new PointF(rect.Width - (Int32)rect.Width / v, (Int32)rect.Height / v),
                            new PointF((Int32)rect.Width / v, rect.Height -(Int32)rect.Height / v),
                            new PointF(rect.Width - (Int32)rect.Width / v, rect.Height - (Int32)rect.Height / v)
                        };

                        var matrix = new System.Drawing.Drawing2D.Matrix();
                        matrix.Translate(0f, 0f);
                        path.Warp(points, rect, matrix, System.Drawing.Drawing2D.WarpMode.Perspective, 0f);

                        // Draw the letter
                        var foreBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                            rect,
                            Color.White,
                            Color.White,
                            System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                        g.FillPath(foreBrush, path);

                    });

                result.Save(_stream, ImageFormat.Bmp);

            }

            return _stream;

        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
        
    }

}
