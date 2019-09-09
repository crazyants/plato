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
            var tableName = PrependTablePrefix(table.Name);

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
                    sb.Append(BuildCreateColumn(column));
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
            var tableName = PrependTablePrefix(table.Name);

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

        public virtual ITableBuilder CreateTableColumns(SchemaTable table)
        {

            var tableName = PrependTablePrefix(table.Name);
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
                        .Append(BuildCreateColumn(column))
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

        public virtual ITableBuilder AlterTableColumns(SchemaTable table)
        {

            var tableName = PrependTablePrefix(table.Name);
            if (table.Columns.Count > 0)
            {
                foreach (var column in table.Columns)
                {
                    var sb = new StringBuilder();
                    if (Options.CheckColumnExistsBeforeAlter)
                    {

                        var normalizedColumnName = column.Name
                            .Replace("[", "")
                            .Replace("]", "");

                        sb.Append("IF EXISTS(SELECT * FROM sys.columns ")
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
                        .Append(" ALTER COLUMN ")
                        .Append(BuildUpdateColumn(column))
                        .Append(";")
                        .Append(NewLine);

                    if (Options.CheckColumnExistsBeforeAlter)
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

            var tableName = PrependTablePrefix(table.Name);
            var sb = new StringBuilder();

            if (table.Columns.Count > 0)
            {

                // -------------
                // Drop contraints for each column first
                // --------------
                foreach (var column in table.Columns)
                {
                    AddStatement(BuildDropColumnConstraints(tableName, column.Name));
                }

                // -------------
                // Next actually drop the columns
                // --------------

                foreach (var column in table.Columns)
                {


                    // Ensure column exists before attempting to drop
                    sb.Append("IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'")
                        .Append(column.Name)
                        .Append("' AND Object_ID = Object_ID(N'")
                        .Append(tableName)
                        .Append("'))")
                        .Append(NewLine)
                        .Append("BEGIN")
                        .Append(NewLine);


                    // Drop the column
                    sb.Append("ALTER TABLE ")
                        .Append(tableName)
                        .Append(" DROP COLUMN ")
                        .Append(column.Name)
                        .Append(";")
                        .Append(NewLine);

                    sb.Append("END")
                        .Append(NewLine);

                }

                AddStatement(sb.ToString());

            }

            return this;
        }

        // ---------------

        private string BuildCreateColumn(SchemaColumn column)
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

        private string BuildUpdateColumn(SchemaColumn column)
        {
            var sb = new StringBuilder();
            sb.Append(column.Name).Append(" ").Append(column.DbTypeNormalized)
                .Append(column.Nullable ? " NULL" : " NOT NULL");
            return sb.ToString();

        }


        string BuildDropColumnConstraints(string tableName, string columnName)
        {

            /*              
                DECLARE @sql NVARCHAR(MAX)
                WHILE 1=1
                BEGIN
                    SELECT TOP 1 @sql = N'alter table tbloffers drop constraint ['+dc.NAME+N']'
                    from sys.default_constraints dc
                    JOIN sys.columns c
                        ON c.default_object_id = dc.object_id
                    WHERE 
                        dc.parent_object_id = OBJECT_ID('tbloffers')
                    AND c.name = N'checkin'
                    IF @@ROWCOUNT = 0 BREAK
                    EXEC (@sql)
                END
            */

            var sb = new StringBuilder();

            // Ensure column exists before attempting to drop
            sb
                .Append("IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'")
                .Append(columnName)
                .Append("' AND Object_ID = Object_ID(N'")
                .Append(tableName)
                .Append("'))")
                .Append(NewLine)
                .Append("BEGIN")
                .Append(NewLine);

            // Drop all contraints for the given table and column
            sb
                .Append("DECLARE @sql NVARCHAR(MAX);")
                .Append(NewLine)
                .Append("WHILE 1 = 1")
                .Append(NewLine)
                .Append("BEGIN")
                .Append(NewLine)
                .Append("SELECT TOP 1 @sql = N'ALTER TABLE ")
                .Append(tableName)
                .Append(" DROP CONSTRAINT [' + dc.NAME + N']' ")
                .Append("FROM sys.default_constraints dc ")
                .Append("JOIN sys.columns c ")
                .Append("ON c.default_object_id = dc.object_id ")
                .Append("WHERE ")
                .Append("dc.parent_object_id = OBJECT_ID('")
                .Append(tableName)
                .Append("') AND c.name = N'")
                .Append(columnName)
                .Append("';")
                .Append(NewLine)
                .Append("IF @@ROWCOUNT = 0 BREAK;")
                .Append(NewLine)
                .Append("EXEC(@sql);")
                .Append(NewLine)
                .Append("END")
                .Append(NewLine);
            
            sb.Append("END")
                 .Append(NewLine);

            return sb.ToString();

        }

    }

}
