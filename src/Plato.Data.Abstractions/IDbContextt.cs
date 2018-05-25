using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Plato.Data.Abstractions.Exceptions;
using Plato.Data.Abstractions.Providers;

namespace Plato.Data.Abstractions
{
    public interface IDbContext : IDisposable
    {

        void Configure(Action<DbContextOptions> options);
   
        DbContextOptions Configuration { get; set; }

        Task<DbDataReader> ExecuteReaderAsync(CommandType commandType, string sql, params object[] commandParams);
        
        T ExecuteScalar<T>(CommandType commandType, string sql, params object[] commandParams);

        Task<T> ExecuteScalarAsync<T>(CommandType commandType, string sql, params object[] commandParams);
        
        void Execute(CommandType commandType, string sql, params object[] commandParams);

        void HandleException(Exception x);

        event DbEventHandlers.DbExceptionEventHandler OnException;



    }

}
