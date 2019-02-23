using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Search.Models
{

    public class FullTextCatalog : IModel<FullTextCatalog>
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public void PopulateModel(IDataReader dr)
        {
            if (dr.ColumnIsNotNull("fulltext_catalog_id"))
                Id = Convert.ToInt32(dr["fulltext_catalog_id"]);

            if (dr.ColumnIsNotNull("name"))
                Name = Convert.ToString(dr["name"]);
            
        }

    }

}
