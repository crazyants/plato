using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data
{
    public interface IDbContextt : IDisposable
    {

        IDataReader ExecuteReader(CommandType commandType, string sql, params object[] commandParams);

        Task<IDataReader> ExecuteReaderAsync(CommandType commandType, string sql, params object[] commandParams);

        //IDataReader ExecuteReader(CommandType commandType, string sql, List<DbParameter> commandParams);

        T ExecuteScalar<T>(CommandType commandType, string sql, params object[] commandParams);

        Task<T> ExecuteScalarAsync<T>(CommandType commandType, string sql, params object[] commandParams);
        
        void Execute(CommandType commandType, string sql, params object[] commandParams);

       //Task ExecuteAsync(CommandType commandType, string sql, params object[] commandParams);


    }
}
