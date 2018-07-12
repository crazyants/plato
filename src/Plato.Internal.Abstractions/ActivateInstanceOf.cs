using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Abstractions
{

    public static class ActivateInstanceOf<T> where T : class
    {
   
        public static readonly Func<T> Instance = Creator();

        static Func<T> Creator()
        {
            Type t = typeof(T);
            if (t == typeof(string))
            {
                return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();
            }
            
            if (t.HasDefaultConstructor())
            {
                return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();
            }
                
            return () => (T)FormatterServices.GetUninitializedObject(t);

        }

    }

}
