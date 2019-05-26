using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Abstractions
{
    public interface IDbContext : IDisposable
    {
    
        Task<T> ExecuteReaderAsync<T>(CommandType commandType, string commandText, Func<DbDataReader, Task<T>> populate, IDbDataParameter[] dbParams = null) where T : class;

        Task<T> ExecuteScalarAsync<T>(CommandType commandType, string commandText, IDbDataParameter[] dbParams = null);

        Task<T> ExecuteNonQueryAsync<T>(CommandType commandType, string commandText, IDbDataParameter[] dbParams = null);

        void Configure(Action<DbContextOptions> options);

        DbContextOptions Configuration { get; }

    }

}
