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

        void HandleException(Exception x);
        
    }

}
