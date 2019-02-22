using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Schemas.Abstractions
{

    public interface ISchemaBuilderBase
    {
        ICollection<string> Statements { get; }

        SchemaBuilderOptions Options { get; }

        ISchemaBuilderBase Configure(Action<SchemaBuilderOptions> configure);

    }

    public interface ISchemaBuilder : ISchemaBuilderBase, IDisposable
    {
       
        ITableBuilder TableBuilder { get; }

        IProcedureBuilder ProcedureBuilder { get; }
        
    }
    
}
