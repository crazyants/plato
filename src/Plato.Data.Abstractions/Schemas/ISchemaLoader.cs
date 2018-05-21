using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Data.Abstractions.Schemas
{
    public interface ISchemaLoader
    {

        List<Schema> Schemas { get; }

        Task<ISchemaLoader> LoadAsync(List<string> versions);
    }
}
