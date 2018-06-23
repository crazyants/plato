using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Plato.Internal.Cache
{

    public interface ICacheDependency
    {
        IChangeToken GetToken(string key);

        void CancelToken(string key);

    }

}
