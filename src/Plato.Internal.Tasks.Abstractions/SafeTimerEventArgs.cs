using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Plato.Internal.Tasks.Abstractions
{
    public class SafeTimerEventArgs : EventArgs
    {

        public IServiceProvider ServiceProvider;

        public SafeTimerEventArgs()
        {
        }

        public SafeTimerEventArgs(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

    }
    
}
