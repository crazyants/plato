using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Plato.Data.Query;

namespace Plato.Data
{
    public interface IDbContext : IDisposable
    {

        DbContextOptions Configuration { get; set; }

        Task<DbDataReader> ExecuteReaderAsync(CommandType commandType, string sql, params object[] commandParams);
        
        T ExecuteScalar<T>(CommandType commandType, string sql, params object[] commandParams);

        Task<T> ExecuteScalarAsync<T>(CommandType commandType, string sql, params object[] commandParams);
        
        void Execute(CommandType commandType, string sql, params object[] commandParams);
        
    }

}
