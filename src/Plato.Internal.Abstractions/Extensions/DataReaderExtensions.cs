using System;
using System.Collections.Generic;
using System.Data;


namespace Plato.Internal.Abstractions.Extensions
{
    public static class DataReaderExtensions
    {
   
        public static bool ColumnIsNotNull(
            this IDataReader dr,
            string columnName)
        {
            return (!object.ReferenceEquals((object)dr[columnName], System.DBNull.Value)) &&
                   ((object)dr[columnName] != null);
        }

        public static bool ColumnIsNotNull(
        this IDataReader dr,
        string columnName,
        IDbModel model)
        {            
            return ColumnIsNotNull(dr, model.GetOrdinal(columnName, dr));
        }
        
        public static bool ColumnIsNotNull(
          this IDataReader dr,
          int columnIndex)
        {
            return (!object.ReferenceEquals((object)dr[columnIndex], System.DBNull.Value)) &&
                   ((object)dr[columnIndex] != null);
        }

        public static TType GetValue<TType>(
            this IDataReader dr,
            string columnName, 
            IDbModel model)
        {
            return (TType) dr[model.GetOrdinal(columnName, dr)];
        }

        public static IEnumerable<T> Select<T>(
        this IDataReader reader, Func<IDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }

    }
}
