﻿using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Abstractions
{
    public interface IDataProvider 
    {
        
        Task<T> ExecuteReaderAsync2<T>(CommandType commandType, string commandText,  Func<DbDataReader, Task<T>> populate, IDbDataParameter[] dbParams) where T : class;

        Task<T> ExecuteScalarAsync2<T>(CommandType commandType, string commandText, IDbDataParameter[] dbParams); 

        Task<T> ExecuteNonQueryAsync2<T>(CommandType commandType, string commandText, IDbDataParameter[] dbParams);
        
        void HandleException(Exception x);
        
    }

}
