using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Plato.Internal.Data.Abstractions.Extensions;

namespace Plato.Internal.Data.Abstractions
{
    
    public class DbCommandParam
    {
        
        public string Name { get; }

        public DbType DbType { get; }

        public string Length { get; set; } = "255";

        public string DbTypeNormalized
        {
            get
            {
                var dbTypeNormalized = this.DbType.ToDbTypeNormalized(this.Length);
                if (String.IsNullOrEmpty(dbTypeNormalized))
                {
                    throw new Exception($"Type not returned for column '{this.Name}' whilst building shema");
                }
                return dbTypeNormalized;
            }
        }

        public object Value { get; set; }

        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        public DbCommandParam(string name)
        {
            this.Name = name;
        }
        public DbCommandParam(string name, object value) : this(name)
        {
            this.Value = value;
        }
        
        public DbCommandParam(
            string name,
            DbType dbType) : this(name, null)
        {
            this.DbType = dbType;
        }

        public DbCommandParam(
            string name,
            object value,
            DbType dbType) : this(name, value)
        {
            this.DbType = dbType;
        }

        public DbCommandParam(
            string name, 
            object value,
            DbType dbType,
            ParameterDirection direction) : this(name, value, dbType)
        {
            this.Direction = direction;
        }

        public DbCommandParam(
            string name,
            DbType dbType,
            ParameterDirection direction) : this(name, null, dbType)
        {
            this.Direction = direction;
        }

        public DbCommandParam(
            DbType dbType,
            ParameterDirection direction) : this("", null, dbType)
        {
            this.Direction = direction;
        }

    }

}
