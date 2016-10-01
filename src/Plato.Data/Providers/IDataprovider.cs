using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data
{
    public interface IDataProvider
    {

        IDataReader ExecuteReader(string sql, params object[] args);

        T ExecuteScalar<T>(string sql, params object[] args);

        int Execute(string sql, params object[] args);

    }
}
