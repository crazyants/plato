using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Abstractions
{
    public interface IDataProvider : IDisposable
    {
    
        Task<DbDataReader> ExecuteReaderAsync(string sql, params object[] args);
        
        Task<T> ExecuteScalarAsync<T>(string sql, params object[] args);

        Task<T> ExecuteNonQueryAsync<T>(string sql, params object[] args);

        void HandleException(Exception x);

        event DbEventHandlers.DbExceptionEventHandler OnException;

    }
}
