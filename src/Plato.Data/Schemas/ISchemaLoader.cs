using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Data.Schemas
{
    public interface ISchemaLoader
    {

        List<SchemaDescriptor> LoadedSchemas { get; }

        void LoadSchemas();

        void LoadSchemas(List<string> versions);
    }
}
