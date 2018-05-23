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

    public class SchemaStoredProcedure
    {
        public SchemaStoredProcedure(
            string name,
            SchemaTable table,
            StoredProcedureType type,
            string key = "")
        {
            this.Name = name;
            this.Table = table;
            this.ProcedureType = type;
            this.Key = key;
        }

        public string Name { get; set; }

        public SchemaTable Table { get; set; }

        public StoredProcedureType ProcedureType { get; set; }

        public string Key { get; set; }
    }
}
