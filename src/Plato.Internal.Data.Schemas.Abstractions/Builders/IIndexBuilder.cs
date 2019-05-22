using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Data.Schemas.Abstractions.Builders
{
    public interface IIndexBuilder : ISchemaBuilderBase
    {

        IIndexBuilder CreateIndex(SchemaIndex index);

        IIndexBuilder AlterIndex(SchemaIndex index);

        IIndexBuilder DropIndex(SchemaIndex index);

    }
    
}
