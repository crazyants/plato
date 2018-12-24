namespace Plato.Internal.Drawing.Abstractions.Letters
{
    public class LetterOptions : BitmapOptions
    {

        public string FontFamily { get; }

        public string Letter { get; set; }
        
        public string BackColor { get; set; }

        public LetterOptions()
        {
            FontFamily = "Trebuchet MS";
            base.Width = 300;
            base.Height = 300;
        }

    }


}
