using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Plato.Data.Abstractions.Schemas
{
    public class SchemaColumn
    {

        private string _tableName;

        public bool PrimaryKey { get; set; }

        public string Name { get; set; }

        public DbType DbType { get; set; }

        public string Length { get; set; }


        private string _defaultValue;

        public string DefaultValue { get; set; }

        public string DefaultValueNormalizsed
        {
            get
            {
                if (!string.IsNullOrEmpty(DefaultValue))
                    return DefaultValue.ToUpper();
                switch (this.DbType)
                {
                    case DbType.Int16:
                        return "0";
                    case DbType.Int32:
                        return "0";
                    case DbType.Int64:
                        return "0";
                    case DbType.Boolean:
                        return "0";
                    case DbType.String:
                        return "''";
                    case DbType.Date:
                        return "GetDate()";
                    case DbType.DateTime2:
                        return "GetDate()";

                }

                throw new Exception($"Type not returned for column '{this.Name}' within table '{_tableName}' whilst building shema");


            }
        }

        public string DbTypeNormalized
        {
            get
            {
                switch (this.DbType)
                {
                    case DbType.Int16:
                        return "SHORT";
                    case DbType.Int64:
                        return "FLOAT?";
                    case DbType.Int32:
                        return "INT";
                    case DbType.Boolean:
                        return "BIT";
                    case DbType.String:
                        return "NVARCHAR(" + this.Length + ")";
                    case DbType.Date:
                        return "DATETIME";
                    case DbType.DateTime2:
                        return "DATETIME2";
                }

                throw new Exception($"Type not returned for column '{this.Name}' within table '{_tableName}' whilst building shema");


            }
        }

        public bool Nullable { get; set; }

    }


}
