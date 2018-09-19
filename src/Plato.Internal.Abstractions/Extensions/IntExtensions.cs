using System;

namespace Plato.Internal.Abstractions.Extensions
{
    public static class IntExtensions
    {

        public static string ToPrettyInt(this int? input, bool precise = false)
        {
            if (input == null)
            {
                return "0";
            }

            return ToPrettyInt((int) input, precise);
        }
        
        public static string ToPrettyInt(this int input, bool precise = false)
        {

            var output = "";
            if (input == 0)
            {
                output = "0";
            }
            else
            {
                if (precise)
                {
                    output = input.ToString("n0");
                }
                else
                {
                    if (input > 1000000)
                    {
                        output = Math.Floor(Decimal.Divide(input, 1000000)).ToString() + "M";
                    }
                    else if (input > 100000)
                    {
                        output = Math.Floor(Decimal.Divide(input, 1000)).ToString("000.0") + "K";
                    }
                    else if (input > 10000)
                    {
                        output = Math.Floor(Decimal.Divide(input, 1000)).ToString("00.0") + "K";
                    }
                    else if (input > 1000)
                    {
                        output = Decimal.Divide(input, 1000).ToString("0.0") + "K";
                    }
                    else
                    {
                        output = input.ToString("n0");
                    }
                }
            }

            return output.Replace(".0K", "K");

        }

        public static int ToSafeCeilingDivision(this int input, int divideBy)
        {
            
            if (input <= 0)
            {
                return default(int);
            }

            if (divideBy <= 0)
            {
                return input;
            }

            var size = decimal.Divide(input, divideBy);
            return System.Convert.ToInt32(System.Math.Ceiling(size));
            
        }

        public static double ToSafeDevision(this int input, int total)
        {

            if (input == 0) return default(int);
            if (total == 0) return input;
            var result = $"{(decimal) input / total:#.##}";
            if (!string.IsNullOrEmpty(result))
            {
                return Convert.ToDouble(result);
            }

            return default(int);

        }


    }
}
