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
        
        Task<T> ExecuteReaderAsync2<T>(CommandType commandType, string sql, Func<DbDataReader, Task<T>> populate, IDbDataParameter[] dbParams = null) where T : class;

        Task<T> ExecuteScalarAsync2<T>(CommandType commandType, string sql, IDbDataParameter[] dbParams = null);

        Task<T> ExecuteNonQueryAsync2<T>(CommandType commandType, string sql, IDbDataParameter[] dbParams = null);




    }

}
