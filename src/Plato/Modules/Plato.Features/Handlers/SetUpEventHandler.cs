using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Features.Handlers
{
    public class SetUpEventHandler : ISetUpEventHandler
    {

        public string Id { get; } = "Plato.Features";

        public string Version { get; } = "1.0.0";

        private readonly ISchemaBuilder _schemaBuilder;
    
        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }

        public async Task SetUp(
            SetUpContext context,
            Action<string, string> reportError)
        {
            
            // --------------------------
            // Build core schemas
            // --------------------------
            
            using (var builder = _schemaBuilder)
            {
                
                // configure
                Configure(builder);

                // entities schema
                Entities(builder);

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


        #region "Private Methods"

        void Configure(ISchemaBuilder builder)
        {

            builder
                .Configure(options =>
                {
                    options.ModuleName = Id;
                    options.Version = Version;
                    options.DropTablesBeforeCreate = true;
                    options.DropProceduresBeforeCreate = true;
                });

        }

        void Entities(ISchemaBuilder builder)
        {

            var features = new SchemaTable()
            {
                Name = "Features",
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
                        Name = "ModuleId",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "Version",
                        DbType = DbType.String,
                        Length = "10"
                    }
                }
            };

            builder
                .CreateTable(features)
                .CreateDefaultProcedures(features);

        }
        
        #endregion


    }

}
