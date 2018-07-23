namespace Plato.Internal.Abstractions.Extensions
{
    public static class LongExtensions
    {

        // Todo: Look at using StringLocializer for this
        private const string Bytes = "bytes";
        private const string Kb = "kb";
        private const string Mb = "mb";
        private const string Gb = "gb";

        public static string ToFriendlyFileSize(this long input)
        {
            
            if (input < 1024)
            {
                return $"{input:N0} " + Bytes;
            }
            else if (input < 1024 * 1024)
            {
                return $"{input / 1024:N2}" + Kb;
            }
            else if (input < 1048576 * 1000)
            {
                return $"{input / (1024 * 1024):N2}" + Mb;
            }

            return $"{input / (1048576 * 1000)}" + Gb;

        }
        
        private static long BytesToMegabytes(this long input)
        {
            return (input / 1024) / 1024;
        }

    }
}
