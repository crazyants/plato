using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Data.Abstractions
{
    public interface IDbContextOptions
    {

        string DatabaseProvider { get; set; }

        string ConnectionString { get; set; }

        string TablePrefix { get; set; }


    }
}
