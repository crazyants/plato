using System;
using System.Collections.Generic;
using System.Data;

namespace Plato.Abstractions.Extensions
{
    public static class DataReaderExtensions
    {

        public static bool ColumnIsNotNull(
            this IDataReader dr,
            string columnNAme)
        {

            if ((!object.ReferenceEquals((object)dr[columnNAme], System.DBNull.Value)) &&
                ((object)dr[columnNAme] != null)) {
                return true;
            }
            return false;
        }
        public static bool ColumnIsNotNull(
          this IDataReader dr,
          int columnIndex)
        {

            if ((!object.ReferenceEquals((object)dr[columnIndex], System.DBNull.Value)) &&
                ((object)dr[columnIndex] != null))
            {
                return true;
            }
            return false;
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
