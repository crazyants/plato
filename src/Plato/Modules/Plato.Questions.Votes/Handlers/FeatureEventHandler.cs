using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Questions.Votes.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
        
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FeatureEventHandler(
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
        }
        
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
                    });

                    builder.TableBuilder.CreateTable(demo);
                    builder.ProcedureBuilder.CreateDefaultProcedures(demo);

                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    context.Errors.Add(error, $"InstallingAsync within {this.GetType().FullName}");
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
        
    }

}
