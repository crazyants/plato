using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Abstractions.Extensions
{
    public static class OjectArrayExtensions
    {


        public static string CombineObjectHashCodesAndGetString(this object[] args)
        {

            if ((args == null) || (args.Length == 0))
            {
                return string.Empty;
            }

            // Precalculate buffer size & ensure GetHashCode()
            // is only ever called once
            var codes = new List<int>();
            var len = 0;
            foreach (var arg in args)
            {
                if (arg != null)
                {
                    var code = arg.GetHashCode();
                    len += code.ToString().Length;
                    codes.Add(code);
                }
            }

            var sb = new StringBuilder(len);
            foreach (var code in codes)
            {
                sb.Append(code);
            }

            codes.Clear();

            return sb.ToString();

        }



    }
}
