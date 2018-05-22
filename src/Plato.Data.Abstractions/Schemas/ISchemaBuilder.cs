using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Data.Abstractions.Schemas
{
    
    public interface ISchemaBuilder
    {

        ISchemaBuilder CreateTable(string tableName, List<SchemaColumn> columns);

    }
}
