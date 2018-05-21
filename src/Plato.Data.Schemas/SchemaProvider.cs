using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plato.Data.Abstractions.Schemas;

namespace Plato.Data.Schemas
{
    public class SchemaProvider : ISchemaProvider
    {
        
        private List<PreparedSchema> _schemas;
        private readonly ISchemaLoader _schemaLoader;

        public SchemaProvider(
            ISchemaLoader schemaLoader)
        {
            _schemaLoader = schemaLoader;
        }

        #region "Implementation"

        public PreparedSchema GetSchema(string version)
        {
            if (_schemas == null)
                LoadSchemas();
            return _schemas.FirstOrDefault(s => s.Version == version);
        }

        public List<PreparedSchema> Schemas
        {
            get
            {
                if (_schemas == null)
                    LoadSchemas();
                return _schemas;
            }
        }

        public ISchemaProvider LoadSchemas()
        {
            return LoadSchemas(new List<string>());
        }

        public ISchemaProvider LoadSchemas(List<string> versions)
        {
            if (_schemas == null)
            {
                var loadedSchemas = _schemaLoader.LoadAsync(versions).Result.Schemas;
                foreach (var version in versions)
                {
                    // adds a dummy schema if the schema version could not be loaded
                    var schema = loadedSchemas.FirstOrDefault(s => s.Version == version);
                    AddPreparedSchema(schema ?? new Schema() { Version = version });
                }
            }

            return this;
        }

        #endregion

        #region "Private Methods"
     
        void AddPreparedSchema(Schema schema)
        {
            if (_schemas == null)
                _schemas = new List<PreparedSchema>();
            _schemas.Add(new PreparedSchema()
            {
                Version = schema.Version,
                InstallStatements = GetStatements(schema.InstallSql),
                UpgradeStatements = GetStatements(schema.UpgradeSql),
                RollbackStatements = GetStatements(schema.RollbackSql),
                TypedVersion = GetTypedVersion(schema.Version)
            });
        }


        const char NewLine = (char) 10;
        const string Go = "GO";

        List<string> GetStatements(string input)
        {

            var seperator = NewLine + Go;
            var output = new List<string>();

            input += seperator;
            if (input.IndexOf(seperator, StringComparison.Ordinal) > -1)
            {
                while (input.IndexOf(seperator, StringComparison.Ordinal) > -1)
                {
                    var sql = input.Substring(0, input.IndexOf(seperator, StringComparison.Ordinal));
                    if (!string.IsNullOrWhiteSpace(sql))
                        output.Add(sql);
                    input = input.Remove(0, (input.IndexOf(seperator, StringComparison.Ordinal) + seperator.Length));
                }
            }
            return output;
        }
        
        Version GetTypedVersion(string input)
        {
            if (!Version.TryParse(input, out var version))
            {
                // Is is a single number?
                if (int.TryParse(input, out var major))
                    return new Version(major + 1, 0, 0);
            }
            if (version.Build != -1)
                return new Version(version.Major, version.Minor, version.Build + 1);
            if (version.Minor != -1)
                return new Version(version.Major, version.Minor + 1, 0);
            return version;
        }

        #endregion


    }
}
