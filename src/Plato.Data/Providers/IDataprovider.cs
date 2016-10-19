using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Plato.Data
{
    public interface IDataProvider : IDisposable
    {

        IDataReader ExecuteReader(string sql, params object[] args);

        Task<IDataReader> ExecuteReaderAsync(string sql, params object[] args);
        
        T ExecuteScalar<T>(string sql, params object[] args);

        Task<T> ExecuteScalarAsync<T>(string sql, params object[] args);
        
        int Execute(string sql, params object[] args);

        void HandleException(Exception x);

        event DbEventHandlers.DbExceptionEventHandler OnException;

    }
}
