using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Reputations.Handlers
{
    public class SetUpEventHandler : BaseSetUpEventHandler
    {

        public string Version { get; } = "1.0.0";

        private readonly SchemaTable _userReputations = new SchemaTable()
        {
            Name = "UserReputations",
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
                    Name = "[Name]",
                    DbType = DbType.String,
                    Length = "255"
                },
                new SchemaColumn()
                {
                    Name = "Points",
                    DbType = DbType.Int32
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
            }
        };
        
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
        }
        
        public override async Task SetUp(SetUpContext context, Action<string, string> reportError)
        {

            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // User reputations
                UserReputations(builder);
                
                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    reportError(error, $"SetUp within {this.GetType().FullName} - {error}");
                }

            }
            
        }

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

        void UserReputations(ISchemaBuilder builder)
        {

            builder.TableBuilder.CreateTable(_userReputations);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_userReputations)
                .CreateProcedure(new SchemaProcedure("SelectUserReputationsPaged", StoredProcedureType.SelectPaged)
                .ForTable(_userReputations)
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
        
    }

}
