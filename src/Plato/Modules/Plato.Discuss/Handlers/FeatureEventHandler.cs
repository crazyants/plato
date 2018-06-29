using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Discuss.Handlers
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

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
  
        public string Version { get; } = "1.0.0";

        private readonly ISchemaBuilder _schemaBuilder;

        public FeatureEventHandler(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }
        
        #region "Implementation"

        public override async Task InstallingAsync(IFeatureEventContext context)
        {
       
            //var schemaBuilder = context.ServiceProvider.GetRequiredService<ISchemaBuilder>();
            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // channels schema
                
                // topics schema
                Topics(builder);
                
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
            //if (!String.Equals(context.Feature.Id, Id, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return Task.CompletedTask;
            //}

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
            //if (!String.Equals(context.Feature.Id, Id, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return Task.CompletedTask;
            //}

            //throw new Exception("This is a test exception from Plato.Demos");

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
            //if (!String.Equals(context.Feature.Id, Id, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return Task.CompletedTask;
            //}

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

        #region "Private Methods"

        void Configure(ISchemaBuilder builder)
        {

            builder
                .Configure(options =>
                {
                    options.ModuleName = ModuleId;
                    options.Version = Version;
                    options.DropTablesBeforeCreate = true;
                    options.DropProceduresBeforeCreate = true;
                });

        }

        void Topics(ISchemaBuilder builder)
        {

            var topics = new SchemaTable()
            {
                Name = "Topics",
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
                        Name = "ParentId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ChannelId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "Title",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "TitleNormalized",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Text]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Data]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsDeleted",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsQueued",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTime
                    }
                }
            };

            builder
                .CreateTable(topics)
                .CreateDefaultProcedures(topics)

                .CreateProcedure(new SchemaProcedure("SelectTopicsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(topics)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Id",
                            DbType = DbType.Int32
                        },
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

        }


        #endregion




    }
}
