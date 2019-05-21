using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Data.Schemas.Abstractions
{
    public class SchemaIndex
    {

        public string TableName { get; set; }

        public string[] Columns { get; set; }

        public short FillFactor { get; set; } = 30;

        public string AutoGenerateName()
        {

            var columnName = string.Empty;
            foreach (var column in Columns)
            {
                columnName = column;
                break;
            }

            var sb = new StringBuilder();
            sb.Append("IX_")
                .Append(this.TableName)
                .Append(columnName);

            return sb.ToString();

        }

    }
    
}
