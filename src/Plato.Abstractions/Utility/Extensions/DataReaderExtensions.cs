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
    }
}
