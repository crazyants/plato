using System.Collections.Generic;

namespace Plato.Internal.Data.Schemas.Abstractions
{
    public class SchemaFullTextIndex
    {

        public string TableName { get; set; }

        public string PrimaryKeyName { get; set; }

        public string[] ColumnNames { get; set; }

        public short FillFactor { get; set; } = 30;

        public string CatalogName { get; set; }

        public int LanguageCode { get; set; } = 1033;

    }

}
