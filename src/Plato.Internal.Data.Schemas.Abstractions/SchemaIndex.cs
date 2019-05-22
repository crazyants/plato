using System.Text;

namespace Plato.Internal.Data.Schemas.Abstractions
{
    public class SchemaIndex
    {
        
        public string TableName { get; set; }

        public string[] Columns { get; set; }

        public short FillFactor { get; set; } = 30;

        public string GenerateName()
        {

            // Get first column
            var columnName = string.Empty;
            foreach (var column in Columns)
            {
                columnName = column;
                break;
            }

            // Auto generate name combining table name with first column name
            var sb = new StringBuilder();
            sb.Append("IX_").Append(this.TableName);
            if (!string.IsNullOrEmpty(columnName))
            {
                sb.Append("_").Append(columnName);
            }
                
            return sb.ToString();

        }

    }
    
}
