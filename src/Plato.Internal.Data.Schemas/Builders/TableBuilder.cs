using System.Text;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Text.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions.Builders;

namespace Plato.Internal.Data.Schemas.Builders
{

    public class TableBuilder : SchemaBuilderBase, ITableBuilder
    {

        public TableBuilder(
            IDbContext dbContext,
            IPluralize pluralize) : base(dbContext, pluralize)
        {
        }

        public virtual ITableBuilder CreateTable(SchemaTable table)
        {
            var tableName = GetTableName(table.Name);

            if (Options.DropTablesBeforeCreate)
            {
                DropTable(table);
            }

            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ")
                .Append(tableName);

            if (table.Columns.Count > 0)
            {
                sb.Append("(");
                sb.Append(NewLine);

                var i = 0;
                var primaryKey = string.Empty;
                foreach (var column in table.Columns)
                {
                    if (column.PrimaryKey)
                        primaryKey = column.Name;
                    sb.Append(DescribeTableColumn(column));
                    if (!string.IsNullOrEmpty(primaryKey))
                    {
                        sb.Append(",");
                    }
                    else
                    {
                        if (i < table.Columns.Count)
                            sb.Append(",");
                    }
                    sb.Append(NewLine);
                    i += 1;
                }

                if (!string.IsNullOrEmpty(primaryKey))
                {
                    sb.Append("CONSTRAINT PK_")
                        .Append(tableName)
                        .Append("_")
                        .Append(primaryKey)
                        .Append(" PRIMARY KEY CLUSTERED ( ")
                        .Append(primaryKey)
                        .Append(" )");
                }

                sb.Append(NewLine);
                sb.Append(");");

            }

            sb.Append("SELECT 1 AS MigrationId;");

            AddStatement(sb.ToString());

            return this;
        }

        public virtual ITableBuilder DropTable(SchemaTable table)
        {
            var tableName = GetTableName(table.Name);

            // drop table ensuring it exists first
            var sb = new StringBuilder();
            sb.Append("IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'")
                .Append(tableName)
                .Append("')")
                .Append(NewLine)
                .Append("BEGIN")
                .Append(NewLine)
                .Append("DROP TABLE ")
                .Append(tableName)
                .Append(NewLine)
                .Append("END");

            AddStatement(sb.ToString());
            return this;
        }
        
        public virtual ITableBuilder AlterTableColumns(SchemaTable table)
        {

            var tableName = GetTableName(table.Name);
            if (table.Columns.Count > 0)
            {
                foreach (var column in table.Columns)
                {
                    var sb = new StringBuilder();
                    if (Options.CheckColumnExistsBeforeCreate)
                    {

                        var normalizedColumnName = column.Name
                            .Replace("[", "")
                            .Replace("]", "");

                        sb.Append("IF NOT EXISTS(SELECT * FROM sys.columns ")
                            .Append("WHERE [Name] = N'")
                            .Append(normalizedColumnName)
                            .Append("' AND Object_ID = Object_ID(N'")
                            .Append(tableName)
                            .Append("'))")
                            .Append(NewLine)
                            .Append("BEGIN")
                            .Append(NewLine);

                    }

                    sb.Append("ALTER TABLE ")
                        .Append(tableName)
                        .Append(" ADD ")
                        .Append(DescribeTableColumn(column))
                        .Append(";")
                        .Append(NewLine);

                    if (Options.CheckColumnExistsBeforeCreate)
                    {
                        sb.Append("END")
                            .Append(NewLine);
                    }

                    sb.Append(NewLine);
                    AddStatement(sb.ToString());

                }
                
            }

            return this;

        }
        
        public virtual ITableBuilder DropTableColumns(SchemaTable table)
        {
            var tableName = GetTableName(table.Name);
            var sb = new StringBuilder();
            if (table.Columns.Count > 0)
            {
                foreach (var column in table.Columns)
                {
                    sb.Append("ALTER TABLE ")
                        .Append(tableName)
                        .Append(" DROP ")
                        .Append(column.Name)
                        .Append(";");
                }

                AddStatement(sb.ToString());

            }
            return this;
        }
        
        // ---------------

        private string DescribeTableColumn(SchemaColumn column)
        {
            var sb = new StringBuilder();
            sb.Append(column.Name)
                .Append(" ")
                .Append(column.DbTypeNormalized);

            if (column.PrimaryKey)
            {
                sb.Append(" IDENTITY(1,1)");
            }
            else
            {
                if (!string.IsNullOrEmpty(column.DefaultValueNormalizsed))
                    sb.Append(" DEFAULT (").Append(column.DefaultValueNormalizsed).Append(")");
            }

            sb.Append(column.Nullable ? " NULL" : " NOT NULL");
            return sb.ToString();

        }

    }
    
}
