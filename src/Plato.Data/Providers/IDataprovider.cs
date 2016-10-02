using System;
using System.Data;

namespace Plato.Data
{
    public interface IDataProvider : IDisposable
    {

        IDataReader ExecuteReader(string sql, params object[] args);

        T ExecuteScalar<T>(string sql, params object[] args);

        int Execute(string sql, params object[] args);

        void HandleException(Exception x);

        event DbEventHandlers.DbExceptionEventHandler OnException;

    }
}
