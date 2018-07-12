using System;
using System.Linq.Expressions;

namespace Plato.Internal.Abstractions
{

    public static class ActivateInstanceOf<T> where T : class
    {
        // https://stackoverflow.com/questions/6582259/fast-creation-of-objects-instead-of-activator-createinstancetype

        public static readonly Func<T> Instance = Expression.Lambda<Func<T>>
        (
            Expression.New(typeof(T))
        ).Compile();

    }

}
