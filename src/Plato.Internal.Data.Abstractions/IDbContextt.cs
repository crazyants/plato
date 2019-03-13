using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Abstractions
{
    public interface IDbContext : IDisposable
    {

        void Configure(Action<DbContextOptions> options);
   
        DbContextOptions Configuration { get; }

        Task<T> ExecuteReaderAsync<T>(CommandType commandType, string sql, Func<DbDataReader, Task<T>> populate, params object[] args) where T : class;

        Task<T> ExecuteScalarAsync<T>(CommandType commandType, string sql, params object[] commandParams);
   
        Task<T> ExecuteNonQueryAsync<T>(CommandType commandType, string sql, params object[] commandParams);
    
    }
    
}
