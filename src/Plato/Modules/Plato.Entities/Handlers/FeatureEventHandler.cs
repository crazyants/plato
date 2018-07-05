using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Entities.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
    
        public string Version { get; } = "1.0.0";
        
        // Entities table
        private readonly SchemaTable _entities = new SchemaTable()
        {
            Name = "Entities",
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
                        Name = "FeatureId",
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
                        Name = "[Message]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Html",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Abstract",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPrivate",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPinned",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsDeleted",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsClosed",
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
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTime
                    }
                }
        };

        // Entity data table
        private readonly SchemaTable _entityData = new SchemaTable()
        {
            Name = "EntityData",
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
                    Name = "EntityId",
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

        // Entity participants
        private readonly SchemaTable _entityParticipants = new SchemaTable()
        {
            Name = "EntityParticipants",
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
                    Name = "EntityId",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "UserId",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "UserName",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "CreatedDate",
                    DbType = DbType.DateTime2
                }
            }
        };

        // Entity replies
        private readonly SchemaTable _entityReplies = new SchemaTable()
        {
            Name = "EntityReplies",
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
                        Name = "EntityId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "[Message]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Html",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Abstract",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPrivate",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPinned",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsDeleted",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsClosed",
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
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTime
                    }
                }
        };
        
        private readonly ISchemaBuilder _schemaBuilder;

        public FeatureEventHandler(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }
        
        #region "Implementation"

        public override async Task InstallingAsync(IFeatureEventContext context)
        {

            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"InstallingAsync called within {ModuleId}");
            
            //var schemaBuilder = context.ServiceProvider.GetRequiredService<ISchemaBuilder>();
            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // Entities schema
                Entities(builder);

                // Entity data schema
                EntityData(builder);

                // Entity participants
                EntityParticipation(builder);

                // Entity replies
                EntityReplies(builder);
                
                // Log statements to execute
                if (context.Logger.IsEnabled(LogLevel.Information))
                {
                    context.Logger.LogInformation($"The following SQL statements will be executed...");
                    foreach (var statement in builder.Statements)
                    {
                        context.Logger.LogInformation(statement);
                    }
                }

                // Execute statements
                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Errors.Add(error.Message, $"InstallingAsync within {this.GetType().FullName}");
                    }

                }

            }
            

        }

        public override Task InstalledAsync(IFeatureEventContext context)
        {
         
            return Task.CompletedTask;

        }

        public override async Task UninstallingAsync(IFeatureEventContext context)
        {

            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"UninstallingAsync called within {ModuleId}");
            
            using (var builder = _schemaBuilder)
            {
                
                // drop entities
                builder
                    .DropTable(_entities)
                    .DropDefaultProcedures(_entities)
                    .DropProcedure(new SchemaProcedure("SelectEntitiesPaged", StoredProcedureType.SelectByKey));
                
                // drop entity data
                builder
                    .DropTable(_entityData)
                    .DropDefaultProcedures(_entityData)
                    .DropProcedure(new SchemaProcedure("SelectEntityDatumByEntityId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityDatumPaged"));
                
                // drop entity participants
                builder
                    .DropTable(_entityParticipants)
                    .DropDefaultProcedures(_entityParticipants)
                    .DropProcedure(new SchemaProcedure("SelectEntityParticipantsPaged"));

                // drop entity replies
                builder
                    .DropTable(_entityReplies)
                    .DropDefaultProcedures(_entityReplies)
                    .DropProcedure(new SchemaProcedure("SelectEntityRepliesPaged", StoredProcedureType.SelectByKey));
                
                // Log statements to execute
                if (context.Logger.IsEnabled(LogLevel.Information))
                {
                    context.Logger.LogInformation($"The following SQL statements will be executed...");
                    foreach (var statement in builder.Statements)
                    {
                        context.Logger.LogInformation(statement);
                    }
                }

                // Execute statements
                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Logger.LogCritical(error.Message, $"An error occurred within the UninstallingAsync method within {this.GetType().FullName}");
                        context.Errors.Add(error.Message, $"UninstallingAsync within {this.GetType().FullName}");
                    }

                }

            }
            
        }

        public override Task UninstalledAsync(IFeatureEventContext context)
        {

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

        void Entities(ISchemaBuilder builder)
        {


            builder
                .CreateTable(_entities)
                .CreateDefaultProcedures(_entities);

            // Overwrite our SelectEntityById created via CreateDefaultProcedures
            // above to also return all EntityData within a second result set
            builder.CreateProcedure(
                new SchemaProcedure(
                        $"SelectEntityById",
                        @" SELECT e.*, 
                                    c.UserName AS CreatedUserName, 
                                    c.NormalizedUserName AS CreatedNormalizedUserName,
                                    c.DisplayName AS CreatedDisplayName,
                                    m.UserName AS ModifiedUserName, 
                                    m.NormalizedUserName AS ModifiedNormalizedUserName,
                                    m.DisplayName AS ModifiedDisplayName
                                FROM {prefix}_Entities e WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users c ON e.CreatedUserId = c.Id
                                    LEFT OUTER JOIN {prefix}_Users m ON e.ModifiedUserId = m.Id
                                WHERE (
                                   e.Id = @Id
                                )
                                SELECT * FROM {prefix}_EntityData WITH (nolock) 
                                WHERE (
                                   EntityId = @Id
                                )")
                    .ForTable(_entities)
                    .WithParameter(_entities.PrimaryKeyColumn));
            
            builder.CreateProcedure(new SchemaProcedure("SelectEntitiesPaged", StoredProcedureType.SelectPaged)
                .ForTable(_entities)
                .WithParameters(new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        Name = "Keywords",
                        DbType = DbType.String,
                        Length = "255"
                    }
                }));

        }

        void EntityData(ISchemaBuilder builder)
        {
            
            builder
                // Create tables
                .CreateTable(_entityData)
                // Create basic default CRUD procedures
                .CreateDefaultProcedures(_entityData)
                .CreateProcedure(new SchemaProcedure("SelectEntityDatumByEntityId", StoredProcedureType.SelectByKey)
                    .ForTable(_entityData)
                    .WithParameter(new SchemaColumn() { Name = "EntityId", DbType = DbType.Int32 }));

            builder.CreateProcedure(new SchemaProcedure("SelectEntityDatumPaged", StoredProcedureType.SelectPaged)
                .ForTable(_entityData)
                .WithParameters(new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        Name = "[Key]",
                        DbType = DbType.String,
                        Length = "255"
                    }
                }));

        }

        void EntityParticipation(ISchemaBuilder builder)
        {


            builder
                .CreateTable(_entityParticipants)
                .CreateDefaultProcedures(_entityParticipants);
            
            builder.CreateProcedure(new SchemaProcedure("SelectEntityParticipantsPaged", StoredProcedureType.SelectPaged)
                .ForTable(_entities)
                .WithParameters(new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        Name = "Id",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "UserName",
                        DbType = DbType.String,
                        Length = "255"
                    }
                }));

        }

        void EntityReplies(ISchemaBuilder builder)
        {
            
            builder
                .CreateTable(_entityReplies)
                .CreateDefaultProcedures(_entityReplies);
            
            builder.CreateProcedure(new SchemaProcedure("SelectEntityRepliesPaged", StoredProcedureType.SelectPaged)
                .ForTable(_entities)
                .WithParameters(new List<SchemaColumn>()
                {
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
