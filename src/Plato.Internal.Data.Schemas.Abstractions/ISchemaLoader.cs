using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Schemas.Abstractions
{
    public interface ISchemaLoader
    {

        List<Schema> Schemas { get; }

        Task<ISchemaLoader> LoadAsync(List<string> versions);
    }
}
