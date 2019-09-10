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
                        Name = "ParentId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "FeatureId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CategoryId",
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
                        Name = "Alias",
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
                        Name = "Urls",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "IsHidden",
                        DbType = DbType.Boolean
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
                        Name = "IsLocked",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsClosed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalViews",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReplies",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalAnswers",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalParticipants",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReactions",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalFollows",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReports",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalStars",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalRatings",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "SummedRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "MeanRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalLinks",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalImages",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalWords",
                        DbType = DbType.Int32
                    },                  
                    new SchemaColumn()
                    {
                        Name = "SortOrder",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV4Address",
                        DbType = DbType.String,
                        Length = "20"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV6Address",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset
                    },
                    new SchemaColumn()
                    {
                        Name = "EditedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "EditedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "LastReplyId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "LastReplyUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "LastReplyDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    }
                }
        };

        // EntityData table
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
                    DbType = DbType.DateTimeOffset
                },
                new SchemaColumn()
                {
                    Name = "CreatedUserId",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "ModifiedDate",
                    DbType = DbType.DateTimeOffset
                },
                new SchemaColumn()
                {
                    Name = "ModifiedUserId",
                    DbType = DbType.Int32
                }
            }
        };
        
        // EntityReplies
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
                        Name = "ParentId",
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
                        Name = "Urls",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "IsHidden",
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
                        Name = "IsAnswer",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReactions",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReports",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalRatings",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "SummedRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "MeanRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalLinks",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalImages",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalWords",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV4Address",
                        DbType = DbType.String,
                        Length = "20"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV6Address",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset
                    },
                    new SchemaColumn()
                    {
                        Name = "EditedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "EditedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    }
                }
        };

        // EntityReplyData table
        private readonly SchemaTable _entityReplyData = new SchemaTable()
        {
            Name = "EntityReplyData",
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
                    Name = "ReplyId",
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
                    DbType = DbType.DateTimeOffset
                },
                new SchemaColumn()
                {
                    Name = "CreatedUserId",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "ModifiedDate",
                    DbType = DbType.DateTimeOffset
                },
                new SchemaColumn()
                {
                    Name = "ModifiedUserId",
                    DbType = DbType.Int32
                }
            }
        };
        
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FeatureEventHandler(
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
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
                
                // Entity replies
                EntityReplies(builder);

                // Entity reply data schema
                EntityReplyData(builder);

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
                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    context.Errors.Add(error, $"InstallingAsync within {this.GetType().FullName}");
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
                builder.TableBuilder.DropTable(_entities);
                
                builder.ProcedureBuilder
                    .DropDefaultProcedures(_entities)
                    .DropProcedure(new SchemaProcedure("SelectEntitiesByFeatureId"))
                    .DropProcedure(new SchemaProcedure("SelectEntitiesPaged"))
                    .DropProcedure(new SchemaProcedure("SelectEntityUsersPaged"));
                
                // drop entity data
                builder.TableBuilder.DropTable(_entityData);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_entityData)
                    .DropProcedure(new SchemaProcedure("SelectEntityDatumByEntityId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityDatumPaged"));
                
                // drop entity replies
                builder.TableBuilder.DropTable(_entityReplies);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_entityReplies)
                    .DropProcedure(new SchemaProcedure("SelectEntityRepliesPaged", StoredProcedureType.SelectByKey));

                // drop entity reply data
                builder.TableBuilder.DropTable(_entityReplyData);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_entityReplyData)
                    .DropProcedure(new SchemaProcedure("SelectEntityReplyDatumByReplyId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityReplyDatumPaged"));
                
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
                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    context.Logger.LogCritical(error, $"An error occurred within the UninstallingAsync method within {this.GetType().FullName}");
                    context.Errors.Add(error, $"UninstallingAsync within {this.GetType().FullName}");
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


            // Tables
            builder.TableBuilder.CreateTable(_entities);
            
            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_entities)
                
                // Overwrite our SelectEntityById created via CreateDefaultProcedures
                // above to also return all EntityData within a second result set
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityById",
                            @"SELECT e.*, f.ModuleId, 0 AS Rank, 0 AS MaxRank,
                                    c.UserName AS CreatedUserName,                              
                                    c.DisplayName AS CreatedDisplayName,                                  
                                    c.Alias AS CreatedAlias,
                                    c.PhotoUrl AS CreatedPhotoUrl,
                                    c.PhotoColor AS CreatedPhotoColor,
                                    c.SignatureHtml AS CreatedSignatureHtml,
                                    c.IsVerified AS CreatedIsVerified,
                                    c.IsStaff AS CreatedIsStaff,
                                    c.IsSpam AS CreatedIsSpam,
                                    c.IsBanned AS CreatedIsBanned,
                                    m.UserName AS ModifiedUserName,                                 
                                    m.DisplayName AS ModifiedDisplayName,                                
                                    m.Alias AS ModifiedAlias,
                                    m.PhotoUrl AS ModifiedPhotoUrl,
                                    m.PhotoColor AS ModifiedPhotoColor,
                                    m.SignatureHtml AS ModifiedSignatureHtml,
                                    m.IsVerified AS ModifiedIsVerified,
                                    m.IsStaff AS ModifiedIsStaff,
                                    m.IsSpam AS ModifiedIsSpam,
                                    m.IsBanned AS ModifiedIsBanned,
                                    l.UserName AS LastReplyUserName,                                
                                    l.DisplayName AS LastReplyDisplayName,                                  
                                    l.Alias AS LastReplyAlias,
                                    l.PhotoUrl AS LastReplyPhotoUrl,
                                    l.PhotoColor AS LastReplyPhotoColor,
                                    l.SignatureHtml AS LastReplySignatureHtml,
                                    l.IsVerified AS LastReplyIsVerified,
                                    l.IsStaff AS LastReplyIsStaff,
                                    l.IsSpam AS LastReplyIsSpam,
                                    l.IsBanned AS LastReplyIsBanned
                                FROM {prefix}_Entities e WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users c ON e.CreatedUserId = c.Id
                                    LEFT OUTER JOIN {prefix}_Users m ON e.ModifiedUserId = m.Id
                                    LEFT OUTER JOIN {prefix}_Users l ON e.LastReplyUserId = l.Id
                                    INNER JOIN {prefix}_ShellFeatures f ON e.FeatureId = f.Id
                                WHERE (
                                   e.Id = @Id
                                );
                                SELECT * FROM {prefix}_EntityData WITH (nolock) 
                                WHERE (
                                   EntityId = @Id
                                );")
                        .ForTable(_entities)
                        .WithParameter(_entities.PrimaryKeyColumn))

                // SelectEntitiesByFeatureId
                .CreateProcedure(new SchemaProcedure("SelectEntitiesByFeatureId",
                        @"SELECT 
                                    e.Id,
                                    e.ParentId,
                                    e.FeatureId,
                                    e.CategoryId,
                                    e.Title,
                                    e.Alias,                                    
                                    e.Abstract,                                    
                                    e.IsHidden,
                                    e.IsPrivate,
                                    e.IsSpam,
                                    e.IsPinned,
                                    e.IsDeleted,
                                    e.IsLocked,
                                    e.IsClosed,
                                    e.TotalViews,
                                    e.TotalReplies,
                                    e.TotalAnswers,
                                    e.TotalParticipants,
                                    e.TotalReactions,
                                    e.TotalFollows,
                                    e.TotalReports,
                                    e.TotalStars,
                                    e.TotalRatings,
                                    e.SummedRating,
                                    e.MeanRating,
                                    e.TotalLinks,
                                    e.TotalImages,
                                    e.TotalWords,          
                                    e.SortOrder,                                    
                                    e.CreatedUserId,
                                    e.CreatedDate,
                                    e.EditedUserId,
                                    e.EditedDate,
                                    e.ModifiedUserId,
                                    e.ModifiedDate,
                                    e.LastReplyId,
                                    e.LastReplyUserId,
                                    e.LastReplyDate,                                    
                                    0 AS [Message], -- not returned for performance reasons
                                    0 AS Html,
                                    0 AS Urls,
                                    0 AS IpV4Address,
                                    0 AS IpV6Address,
                                    0 AS Rank,
                                    0 AS MaxRank,
                                    f.ModuleId, 
                                    c.UserName AS CreatedUserName,                              
                                    c.DisplayName AS CreatedDisplayName,                                  
                                    c.Alias AS CreatedAlias,
                                    c.PhotoUrl AS CreatedPhotoUrl,
                                    c.PhotoColor AS CreatedPhotoColor,
                                    c.SignatureHtml AS CreatedSignatureHtml,
                                    c.IsVerified AS CreatedIsVerified,
                                    c.IsStaff AS CreatedIsStaff,
                                    c.IsSpam AS CreatedIsSpam,
                                    c.IsBanned AS CreatedIsBanned,
                                    m.UserName AS ModifiedUserName,                                 
                                    m.DisplayName AS ModifiedDisplayName,                                
                                    m.Alias AS ModifiedAlias,
                                    m.PhotoUrl AS ModifiedPhotoUrl,
                                    m.PhotoColor AS ModifiedPhotoColor,
                                    m.SignatureHtml AS ModifiedSignatureHtml,
                                    m.IsVerified AS ModifiedIsVerified,
                                    m.IsStaff AS ModifiedIsStaff,
                                    m.IsSpam AS ModifiedIsSpam,
                                    m.IsBanned AS ModifiedIsBanned,
                                    l.UserName AS LastReplyUserName,                                
                                    l.DisplayName AS LastReplyDisplayName,                                  
                                    l.Alias AS LastReplyAlias,
                                    l.PhotoUrl AS LastReplyPhotoUrl,
                                    l.PhotoColor AS LastReplyPhotoColor,
                                    l.SignatureHtml AS LastReplySignatureHtml,
                                    l.IsVerified AS LastReplyIsVerified,
                                    l.IsStaff AS LastReplyIsStaff,
                                    l.IsSpam AS LastReplyIsSpam,
                                    l.IsBanned AS LastReplyIsBanned
                                FROM {prefix}_Entities e WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users c ON e.CreatedUserId = c.Id
                                    LEFT OUTER JOIN {prefix}_Users m ON e.ModifiedUserId = m.Id
                                    LEFT OUTER JOIN {prefix}_Users l ON e.LastReplyUserId = l.Id
                                    INNER JOIN {prefix}_ShellFeatures f ON e.FeatureId = f.Id
                                WHERE (
                                   e.FeatureId = @FeatureId
                                )")
                    .ForTable(_entities)
                    .WithParameter(new SchemaColumn() { Name = "FeatureId", DbType = DbType.Int32 }))
                    
                .CreateProcedure(new SchemaProcedure("SelectEntitiesPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_entities)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }))

                .CreateProcedure(new SchemaProcedure("SelectEntityUsersPaged", StoredProcedureType.SelectPaged)
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

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _entities.Name,
                Columns = new string[]
                {
                    "ParentId",
                    "FeatureId",
                    "CategoryId",
                    "SortOrder",
                    "IsHidden",
                    "IsPrivate",
                    "IsSpam",
                    "IsPinned",
                    "IsDeleted",
                    "CreatedUserId",
                    "CreatedDate",
                    "ModifiedUserId",
                    "ModifiedDate",
                    "LastReplyUserId",
                    "LastReplyDate"
                }
            });


        }

        void EntityData(ISchemaBuilder builder)
        {

            // Tables
            builder.TableBuilder.CreateTable(_entityData);
            
            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_entityData)

                .CreateProcedure(new SchemaProcedure("SelectEntityDatumByEntityId", StoredProcedureType.SelectByKey)
                    .ForTable(_entityData)
                    .WithParameter(new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("SelectEntityDatumPaged", StoredProcedureType.SelectPaged)
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

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _entityData.Name,
                Columns = new string[]
                {
                    "EntityId",
                    "CreatedUserId",
                    "CreatedDate"
                }
            });

        }
        
        void EntityReplies(ISchemaBuilder builder)
        {
            
            // Tables
            builder.TableBuilder.CreateTable(_entityReplies);

            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_entityReplies)

                // Overwrite our SelectEntityReplyById created via CreateDefaultProcedures
                // above to also return basic user data & reply meta data
                .CreateProcedure(
                    new SchemaProcedure(
                            "SelectEntityReplyById",
                            @"SELECT r.*, 
                                    c.UserName AS CreatedUserName,                                   
                                    c.DisplayName AS CreatedDisplayName,                                 
                                    c.Alias AS CreatedAlias,
                                    c.PhotoUrl AS CreatedPhotoUrl,
                                    c.PhotoColor AS CreatedPhotoColor,
                                    c.SignatureHtml AS CreatedSignatureHtml,
                                    c.IsVerified AS CreatedIsVerified,
                                    c.IsStaff AS CreatedIsStaff,
                                    c.IsSpam AS CreatedIsSpam,
                                    c.IsBanned AS CreatedIsBanned,
                                    m.UserName AS ModifiedUserName,                                    
                                    m.DisplayName AS ModifiedDisplayName,                                
                                    m.Alias AS ModifiedAlias,
                                    m.PhotoUrl AS ModifiedPhotoUrl,
                                    m.PhotoColor AS ModifiedPhotoColor,
                                    m.SignatureHtml AS ModifiedSignatureHtml,
                                    m.IsVerified AS ModifiedIsVerified,
                                    m.IsStaff AS ModifiedIsStaff,
                                    m.IsSpam AS ModifiedIsSpam,
                                    m.IsBanned AS ModifiedIsBanned
                                FROM {prefix}_EntityReplies r WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users c ON r.CreatedUserId = c.Id
                                    LEFT OUTER JOIN {prefix}_Users m ON r.ModifiedUserId = m.Id
                                WHERE (
                                   r.Id = @Id
                                );
                                SELECT * FROM {prefix}_EntityReplyData WITH (nolock) 
                                WHERE (
                                   ReplyId = @Id
                                );")
                        .ForTable(_entityReplies)
                        .WithParameter(_entityReplies.PrimaryKeyColumn))

                .CreateProcedure(new SchemaProcedure("SelectEntityRepliesPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_entityReplies)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _entityReplies.Name,
                Columns = new string[]
                {
                    "EntityId",
                    "CreatedUserId",
                    "CreatedDate"
                }
            });

        }

        void EntityReplyData(ISchemaBuilder builder)
        {

            // Tables
            builder.TableBuilder.CreateTable(_entityReplyData);

            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_entityReplyData)

                .CreateProcedure(new SchemaProcedure("SelectEntityReplyDatumByReplyId", StoredProcedureType.SelectByKey)
                    .ForTable(_entityReplyData)
                    .WithParameter(new SchemaColumn() { Name = "ReplyId", DbType = DbType.Int32 }))

                .CreateProcedure(new SchemaProcedure("SelectEntityReplyDatumPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_entityReplyData)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "[Key]",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _entityReplyData.Name,
                Columns = new string[]
                {
                    "ReplyId",
                    "CreatedUserId",
                    "CreatedDate"
                }
            });

        }

        #endregion

    }

}
