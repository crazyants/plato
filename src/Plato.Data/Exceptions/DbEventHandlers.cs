using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data
{
    public class DbEventHandlers
    {

        public delegate void DbExceptionEventHandler(
            object sender, DbExceptionEventArgs e);

    }
}
