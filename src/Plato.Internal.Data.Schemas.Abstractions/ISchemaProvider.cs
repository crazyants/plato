using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Data.Schemas.Abstractions
{
    public interface ISchemaProvider
    {

        PreparedSchema GetSchema(string version);

        List<PreparedSchema> Schemas { get; }

        ISchemaProvider LoadSchemas();

        ISchemaProvider LoadSchemas(List<string> versions);

    }
}
