using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Plato.Data.Abstractions.Exceptions;

namespace Plato.Data.Abstractions.Providers
{
    public interface IDataProvider : IDisposable
    {
        IDbConnection Connection { get; }

        IDataReader ExecuteReader(string sql, params object[] args);

        Task<DbDataReader> ExecuteReaderAsync(string sql, params object[] args);
        
        T ExecuteScalar<T>(string sql, params object[] args);

        Task<T> ExecuteScalarAsync<T>(string sql, params object[] args);

        int Execute(string sql, params object[] args);

        Task<int> ExecuteAsync(string sql, params object[] args);

        void HandleException(Exception x);

        event DbEventHandlers.DbExceptionEventHandler OnException;

    }
}
