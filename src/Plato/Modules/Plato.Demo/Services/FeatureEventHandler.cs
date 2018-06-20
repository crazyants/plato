using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features;

namespace Plato.Demo.Services
{

    // Feature event handlers are executed in a temporary shell context 
    // This is necessary as the feature may not be enabled and as 
    // such the event handlers for the feature won't be registered with DI
    // For example we can't invoke the Installing or Installed events within
    // the main context as the feature is currently disabled within this context
    // so the IFeatureEventHandler provider for the feature has not been registered within DI.
    // ShellFeatureManager instead creates a temporary context consisting of a shell descriptor
    // with the features we want to enable or disable. The necessary IFeatureEventHandler can
    // then be registered within DI for the features we are enabling or disabling and the events can be invoked.

    public class FeatureEventHandler : IFeatureEventHandler
    {

        public string Id { get; } = "Plato.Demo";
        

        private readonly ISchemaBuilder _schemaBuilder;

        public FeatureEventHandler(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }




        public async Task InstallingAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, Id, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }


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
                        options.ModuleName = "Plato.Demo";
                        options.Version = "1.0.0";
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

        public Task InstalledAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, Id, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            try
            {
                
             
                
            }
            catch (Exception e)
            {
                context.Errors.Add(context.Feature.Id, e.Message);
            }

            return Task.CompletedTask;

        }

        public Task UninstallingAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, Id, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            //throw new Exception("This is a test exception from Plato.Demos");

            try
            {


            }
            catch (Exception e)
            {
                context.Errors.Add(context.Feature.Id, e.Message);
            }

            return Task.CompletedTask;

        }

        public Task UninstalledAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, Id, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            try
            {


            }
            catch (Exception e)
            {
                context.Errors.Add(context.Feature.Id, e.Message);
            }

            return Task.CompletedTask;
        }
    }
}
