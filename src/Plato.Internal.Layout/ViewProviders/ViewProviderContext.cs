using System;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Internal.Layout.ViewProviders
{

    public interface IViewProviderContext
    {

        IUpdateModel Updater { get; set; }

        Controller Controller { get; }
        
    }

    public class ViewProviderContext : IViewProviderContext
    {

        public IUpdateModel Updater { get; set; }

        public Controller Controller { get; }
        
        public ViewProviderContext(IUpdateModel updater)
        {
            Updater = updater ?? throw new ArgumentNullException(nameof(updater));
            Controller = updater as Controller ?? throw new Exception($"Could not convert type of '{this.GetType()}' to type of 'Controller'.");
        }
    }
}
