using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Data;

namespace Plato.Shell.Models
{
    public class ShellContext : IDisposable
    {

        private bool _disposed = false;

        public bool IsActivated;

        public ShellSettings Settings { get; set; }

        public IServiceProvider ServiceProvider { get; set; }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }

                // Disposes all the services registered for this shell
                //(ServiceProvider as IDisposable).Dispose();
                IsActivated = false;

                _disposed = true;
            }
        }
        
        public IServiceScope CreateServiceScope()
        {
            return ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }
           
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        ~ShellContext()
        {
            Dispose(false);
        }

        #endregion

    }
}
