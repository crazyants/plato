using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features;
using Plato.Internal.Features.Abstractions;

namespace Plato.Mentions.Handlers
{
    
    public class FeatureEventHandler : BaseFeatureEventHandler
    {

        #region "Constructor"

        private readonly ISchemaBuilder _schemaBuilder;

        public FeatureEventHandler(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }

        #endregion

        #region "Implementation"

        public override async Task InstallingAsync(IFeatureEventContext context)
        {
      
            var demo = new SchemaTable()
            {
                Name = "Demo",
                Columns = new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        PrimaryKey = true,
                        Name = "Id",
                        DbType = DbType.Int32
                    }
                }
            };

            //var schemaBuilder = context.ServiceProvider.GetRequiredService<ISchemaBuilder>();
            using (var builder = _schemaBuilder)
            {

                // create tables and default procedures
                builder
                    .Configure(options =>
                    {
                        options.ModuleName = base.ModuleId;
                        options.Version = "1.0.0";
                        options.DropTablesBeforeCreate = true;
                        options.DropProceduresBeforeCreate = true;
                    })
                    // Create tables
                    .CreateTable(demo)
                    // Create basic default CRUD procedures
                    .CreateDefaultProcedures(demo);

                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Errors.Add(error.Message, $"InstallingAsync within {this.GetType().FullName}");
                    ;
                }

                }

            }
            
        }

        public override Task InstalledAsync(IFeatureEventContext context)
        {
      
            try
            {
                
             
                
            }
            catch (Exception e)
            {
                context.Errors.Add(context.Feature.ModuleId, e.Message);
            }

            return Task.CompletedTask;

        }

        public override Task UninstallingAsync(IFeatureEventContext context)
        {
   
            try
            {


            }
            catch (Exception e)
            {
                context.Errors.Add(context.Feature.ModuleId, e.Message);
            }

            return Task.CompletedTask;

        }

        public override Task UninstalledAsync(IFeatureEventContext context)
        {
       
            try
            {


            }
            catch (Exception e)
            {
                context.Errors.Add(context.Feature.ModuleId, e.Message);
            }

            return Task.CompletedTask;
        }

        #endregion


    }
}
