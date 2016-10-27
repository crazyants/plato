using System;

namespace Plato.Data
{
    public class DbExceptionEventArgs : EventArgs
    {

        internal Exception _e;

        public Exception Exception
        {
            get
            {
                return _e;
            }
        }

        protected DbExceptionEventArgs()
        {

        }

        public DbExceptionEventArgs(Exception e)
        {
            _e = e;
        }

    }
}
