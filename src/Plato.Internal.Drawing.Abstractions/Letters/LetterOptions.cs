namespace Plato.Internal.Drawing.Abstractions.Letters
{
    public class LetterOptions : BitmapOptions
    {

        public string Letter { get; set; }
        
        public string BackColor { get; set; }

        public LetterOptions()
        {
            base.Width = 300;
            base.Height = 300;
        }

    }


}
