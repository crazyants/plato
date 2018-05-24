using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Data.Abstractions.Schemas
{

    // TODO: Split enum into seperate more extensible classes
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
        
        public List<SchemaColumn> Parameters { get; private set; }

        public SchemaProcedure ForTable(SchemaTable table)
        {
            this.Table = table;
            return this;
        }
        
        public SchemaProcedure WithParameter(SchemaColumn parameter)
        {
            if (this.Parameters == null)
                this.Parameters = new List<SchemaColumn>();
            this.Parameters.Add(parameter);
            return this;
        }

        public SchemaProcedure WithParameters(List<SchemaColumn> parameters)
        {
            this.Parameters = parameters;
            return this;
        }
        
    }
}
