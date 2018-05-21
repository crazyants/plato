using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Data.Abstractions.Schemas
{
    public interface ISchemaParser
    {
        IEnumerable<string> Parse(string input);
    }
}
