using System;

namespace Plato.Data
{
    public class DbExceptionEventArgs : EventArgs
    {

        internal Exception _x;

        public Exception Exception
        {
            get
            {
                return _x;
            }
        }

        protected DbExceptionEventArgs()
        {

        }

        public DbExceptionEventArgs(Exception x)
        {
            _x = x;
        }

    }
}
