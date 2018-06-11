using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Plato.Internal.Abstractions.Extensions
{
    public static class ExceptionExtensions
    {

        public static bool IsFatal(this Exception ex)
        {
            return
                ex is OutOfMemoryException ||
                ex is SecurityException ||
                ex is SEHException;
        }
    }
}
