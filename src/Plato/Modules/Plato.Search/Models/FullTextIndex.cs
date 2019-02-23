using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Search.Models
{

    public class FullTextIndex : IModel<FullTextIndex>
    {

        public int Id { get; set; }

        public int CatalogId { get; set; }

        public string TableName { get; set; }

        public string CatalogName { get; set; }

        public string UniqueIndexName { get; set; }

        public string ColumnName { get; set; }
        
        public int ColumnId { get; set; }

        public void PopulateModel(IDataReader dr)
        {
            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("CatalogId"))
                Id = Convert.ToInt32(dr["CatalogId"]);

            if (dr.ColumnIsNotNull("TableName"))
                TableName = Convert.ToString(dr["TableName"]);

            if (dr.ColumnIsNotNull("CatalogName"))
                CatalogName = Convert.ToString(dr["CatalogName"]);

            if (dr.ColumnIsNotNull("UniqueIndexName"))
                UniqueIndexName = Convert.ToString(dr["UniqueIndexName"]);

            if (dr.ColumnIsNotNull("ColumnName"))
                ColumnName = Convert.ToString(dr["ColumnName"]);

            if (dr.ColumnIsNotNull("ColumnId"))
                ColumnId = Convert.ToInt32(dr["ColumnId"]);

        }

    }

}
