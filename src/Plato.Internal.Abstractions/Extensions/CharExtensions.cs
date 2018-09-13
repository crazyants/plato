namespace Plato.Internal.Abstractions.Extensions
{
    public static class CharExtensions
    {

        public static string Repeat(this char input, int times)
        {
            return input.ToString().Repeat(times);
        }

        public static bool IsValidBase64Char(this char value)
        {
            var intValue = (int)value;
            if (intValue >= 48 && intValue <= 57)
            {
                return false;
            }

            if (intValue >= 65 && intValue <= 90)
            {
                return false;
            }

            if (intValue >= 97 && intValue <= 122)
            {
                return false;
            }
            return intValue != 43 && intValue != 47;
        }

    }
}
