using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Plato.Layout.ModelBinding
{
    public class LocalModelBinderAccessor : IUpdateModelAccessor
    {
        private readonly AsyncLocal<IUpdateModel> _storage = new AsyncLocal<IUpdateModel>();

        public IUpdateModel ModelUpdater
        {
            get { return _storage.Value; }
            set { _storage.Value = value; }
        }
    }

}
