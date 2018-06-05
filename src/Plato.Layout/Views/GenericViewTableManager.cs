using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Plato.Layout.Views
{

    public interface IGenericViewTableManager
    {

    }

    public class GenericViewTableManager : IGenericViewTableManager
    {

        private static ConcurrentDictionary<string, GenericViewDescriptor> _viewDescriptors =
            new ConcurrentDictionary<string, GenericViewDescriptor>();




    }

}
