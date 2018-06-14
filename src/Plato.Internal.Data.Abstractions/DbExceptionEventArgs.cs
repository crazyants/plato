using System;

namespace Plato.Internal.Data.Abstractions
{
    public class DbExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; }

        protected DbExceptionEventArgs()
        {
        }

        public DbExceptionEventArgs(Exception e)
        {
            Exception = e;
        }

    }
}
