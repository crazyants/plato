using System.Collections.Generic;

namespace Plato.Internal.Data.Schemas.Abstractions
{
    public class SchemaTable
    {

        public string Name { get; set; }

        /// <summary>
        /// Returns a version of the column Name safe for inclusion within @paramter arguments. 
        /// </summary>
        public string NameNormalized => this.Name.Replace("[", "").Replace("]", "");
        
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
