using System.Drawing;

namespace Plato.Internal.Drawing.Extensions
{
    public static class StringExtensions
    {
    
        public static Color ToColor(this string hex, Color fallbackColor)
        {

            if (string.IsNullOrEmpty(hex))
            {
                return fallbackColor;
            }

            if (!hex.IsHex())
            {
                return fallbackColor;
            }

            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Color));
            var color = converter.ConvertFromString(hex.StartsWith("#") ? hex : '#' + hex);
            if (color is Color color1)
            {
                return color1;
            }

            return fallbackColor;
        }

        public static bool IsHex(this string input)
        {

            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            
            // Check bounds (#RRGGBB = 7, RRGGBB = 6)
            if (input.StartsWith("#"))
            {
                if (input.Length > 7) { return false; }
            }
            else
            {
                if (input.Length > 6) { return false; }
            }
           
            // Convert to lower so we only need to check for lowercase chars
            foreach (var c in input.ToLower().ToCharArray())
            {
                var isHex = ((c == '#') ||
                             (c >= '0' && c <= '9') ||
                             (c >= 'a' && c <= 'f'));
                if (!isHex)
                {
                    return false;
                }
            }

            return true;

        }

    }

}
