using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Plato.Data
{
    public interface IDataProvider : IDisposable
    {
        IDbConnection Connection { get; }

        IDataReader ExecuteReader(string sql, params object[] args);

        Task<DbDataReader> ExecuteReaderAsync(string sql, params object[] args);
        
        T ExecuteScalar<T>(string sql, params object[] args);

        Task<T> ExecuteScalarAsync<T>(string sql, params object[] args);

        int Execute(string sql, params object[] args);

        void HandleException(Exception x);

        event DbEventHandlers.DbExceptionEventHandler OnException;

    }
}
