using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Shell.Models
{
    public class ShellContext : IDisposable
    {

        private bool _disposed = false;

        public bool IsActivated;

        public ShellSettings Settings { get; set; }

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

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ShellContext() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
        
        ~ShellContext()
        {
            Dispose(false);
        }

        #endregion

    }
}
