using System;

namespace Plato.Internal.Abstractions.Extensions
{
    public static class TypeExtensions
    {

        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }

    }
}
