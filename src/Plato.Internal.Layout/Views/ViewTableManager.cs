using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Plato.Internal.Layout.Views
{

    //public interface IViewTableManager
    //{
    //    ViewDescriptor TryAdd(IView view);
    //}

    //public class ViewTableManager : IViewTableManager
    //{
        
    //    private static readonly ConcurrentDictionary<string, ViewDescriptor> _views =
    //        new ConcurrentDictionary<string, ViewDescriptor>();

    //    public ViewDescriptor TryAdd(IView view)
    //    {
            
    //        // TODO: Do we need this? 
    //        var descriptor = new ViewDescriptor()
    //        {
    //            Name = view.ViewName,
    //            View = view
    //        };

    //        _views.TryAdd(view.ViewName, descriptor);

    //        return descriptor;

    //    }

    //    bool IsViewModelAnonymousType(object model)
    //    {

    //        // We need a model to inspect
    //        if (model == null)
    //        {
    //            return false;
    //        }

    //        object[] attrs = model
    //            .GetType()
    //            .GetCustomAttributes(typeof(CompilerGeneratedAttribute), true);
    //        return attrs != null && attrs.Length > 0;

    //    }

    //}

}
