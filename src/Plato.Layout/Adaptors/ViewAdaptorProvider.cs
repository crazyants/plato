using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace Plato.Layout.Adaptors
{

    public interface IViewAdaptorProvider
    {
        
        string ViewName { get; }

        Task<IViewAdaptorResult> ConfigureAsync();
    }
    
}
