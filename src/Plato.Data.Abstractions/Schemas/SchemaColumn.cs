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

        /// <summary>
        /// Returns a version of the column Name safe for inclusion within @paramter arguments. 
        /// </summary>
        public string NameNormalized => this.Name.Replace("[", "").Replace("]", "");

        public DbType DbType { get; set; }

        public string Length { get; set; }


        private string _defaultValue;

        public string DefaultValue { get; set; }

        /// <summary>
        /// Sets a default value for the column based on the columns data type. 
        /// </summary>
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
        
        /// <summary>
        /// Returns a version of the column type safe for inclusion within @paramter arguments. 
        /// </summary>
        public string DbTypeNormalized
        {
            get
            {
                switch (this.DbType)
                {
                    case DbType.Int16:
                        return "short";
                    case DbType.Int64:
                        return "float";
                    case DbType.Int32:
                        return "int";
                    case DbType.Boolean:
                        return "bit";
                    case DbType.String:
                        return "nvarchar(" + this.Length + ")";
                    case DbType.Date:
                        return "datetime";
                    case DbType.DateTime2:
                        return "datetime2";
                }

                throw new Exception($"Type not returned for column '{this.Name}' within table '{_tableName}' whilst building shema");


            }
        }

        public bool Nullable { get; set; }

    }


}
