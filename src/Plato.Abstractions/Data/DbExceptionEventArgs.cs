using System;

namespace Plato.Abstractions.Data
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
