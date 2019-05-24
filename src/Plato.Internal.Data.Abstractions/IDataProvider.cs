using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Abstractions
{
    public interface IDataProvider 
    {

        Task<T> ExecuteReaderAsync<T>(string sql, Func<DbDataReader, Task<T>> populate, params object[] args) where T : class;
        
        Task<T> ExecuteScalarAsync<T>(string sql, params object[] args);

        Task<T> ExecuteNonQueryAsync<T>(string sql, params object[] args);
        
        // --- Testing

        Task<T> ExecuteReaderAsync2<T>(string sql, Func<DbDataReader, Task<T>> populate, DbParam[] dbParams) where T : class;

        Task<T> ExecuteScalarAsync2<T>(string sql, DbParam[] dbParams);

        Task<T> ExecuteNonQueryAsync2<T>(string sql, DbParam[] dbParams);
        

        void HandleException(Exception x);
        
    }

}
