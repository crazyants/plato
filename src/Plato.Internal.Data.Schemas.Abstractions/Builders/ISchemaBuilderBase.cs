using System;
using System.Collections.Generic;

namespace Plato.Internal.Data.Schemas.Abstractions.Builders
{
    public interface ISchemaBuilderBase : IDisposable
    {

        ICollection<string> Statements { get; }
        
        ISchemaBuilderBase Configure(Action<SchemaBuilderOptions> configure);

    }

}
