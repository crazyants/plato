using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

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


        public static string ToIdealTextColor(this string hex)
        {

            var fallback = "ffffff";

            if (string.IsNullOrEmpty(hex))
            {
                return fallback;
            }

            // Validate
            if (!hex.IsHex())
            {
                return fallback;
            }

            var threshold = 127;
            var components = hex.ToRgbComponents();
            var delta = (components["r"] * 0.299) +
                        (components["g"] * 0.587) +
                        (components["b"] * 0.114);

            return ((255 - Math.Floor(delta)) < threshold) ? "000000" : "ffffff";

        }

        public static IDictionary<string, int> ToRgbComponents(this string hex)
        {

            // Remove # suffix
            if (hex.StartsWith("#"))
            {
                hex = hex.Substring(1, hex.Length - 1);
            }

            // Break apart
            var r = hex.Substring(0, 2);
            var g = hex.Substring(2, 2);
            var b = hex.Substring(4, 2);

            // Reassemble 
            var sb = new StringBuilder();
            sb
                .Append(r)
                .Append(b)
                .Append(b);

            // Validate again
            if (!sb.ToString().IsHex())
            {
                return new Dictionary<string, int>()
                {
                    ["r"] = 0,
                    ["g"] = 0,
                    ["b"] = 0
                };
            }
            
            return new Dictionary<string, int>()
            {
                ["r"] = Convert.ToInt32(r, 16),
                ["g"] = Convert.ToInt32(r, 16),
                ["b"] = Convert.ToInt32(r, 16)
            };

        }

    }

}
