using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Plato.Data.Abstractions.Schemas
{
    public class SchemaTable
    {

        public string Name { get; set; }

        public List<SchemaColumn> Columns { get; set; }

        public SchemaColumn PrimaryKeyColumn
        {
            get
            {
                foreach (var column in this.Columns)
                {
                    if (column.PrimaryKey)
                        return column;
                }
                return null;
            }
        }

    }
}
