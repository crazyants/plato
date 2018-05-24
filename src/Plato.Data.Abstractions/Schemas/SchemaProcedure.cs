using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Data.Abstractions.Schemas
{
    public enum StoredProcedureType
    {
        InsertUpdate,
        Select,
        SelectPaged,
        SelectByKey,
        DeleteByKey
    }

    public class SchemaProcedure
    {
        public SchemaProcedure(
            string name,
            StoredProcedureType type)
        {
            this.Name = name;
            this.ProcedureType = type;
        }

        public string Name { get; set; }

        public StoredProcedureType ProcedureType { get; set; }
        
        public SchemaTable Table { get; private set; }
        
        public SchemaColumn Key { get; private set; }

        public SchemaProcedure ForTable(SchemaTable table)
        {
            this.Table = table;
            return this;
        }
        
        public SchemaProcedure WithKey(SchemaColumn key)
        {
            this.Key = key;
            return this;
        }

    }
}
