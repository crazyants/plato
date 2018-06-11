using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Data.Schemas.Abstractions
{
    public interface ISchemaParser
    {
        IEnumerable<string> Parse(string input);
    }
}
