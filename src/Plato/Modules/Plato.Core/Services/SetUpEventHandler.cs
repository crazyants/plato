using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Core.Services
{
    public class SetUpEventHandler : ISetUpEventHandler
    {

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISiteSettingsStore _siteSettingsStore;
    
        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder,
            ISiteSettingsStore siteSettingsService)
        {
            _schemaBuilder = schemaBuilder;
            _siteSettingsStore = siteSettingsService;
        }

        public async Task SetUp(
            SetUpContext context,
            Action<string, string> reportError)
        {
            
            // --------------------------
            // Build core schemas
            // --------------------------

            var dictionaryTable = new SchemaTable()
            {
                Name = "DictionaryEntries",
                Columns = new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        PrimaryKey = true,
                        Name = "Id",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "[Key]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Value]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    }
                }
            };

            var documentTable = new SchemaTable()
            {
                Name = "DocumentEntries",
                Columns = new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        PrimaryKey = true,
                        Name = "Id",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "[Type]",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Value]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    }
                }
            };
            
            using (var builder = _schemaBuilder)
            {

                // build dictionary store

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = "Plato.Core";
                        options.Version = "1.0.0";
                    })
                    .CreateTable(dictionaryTable)
                    .CreateDefaultProcedures(dictionaryTable)
                    .CreateProcedure(new SchemaProcedure("SelectDictionaryEntryByKey", StoredProcedureType.SelectByKey)
                        .ForTable(dictionaryTable).WithParameter(new SchemaColumn() {Name = "[Key]", DbType = DbType.Int32}));

                // build document store

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = "Plato.Core";
                        options.Version = "1.0.0";
                    })
                    .CreateTable(documentTable)
                    .CreateDefaultProcedures(documentTable)
                    .CreateProcedure(new SchemaProcedure("SelectDocumentEntryByType", StoredProcedureType.SelectByKey)
                        .ForTable(documentTable)
                        .WithParameter(new SchemaColumn() {Name = "[Type]", Length = "500", DbType = DbType.String}));

                // Did any errors occur?

                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        reportError(error.Message, error.StackTrace);
                    }
                 
                }
            }
            
        }

    }

}
