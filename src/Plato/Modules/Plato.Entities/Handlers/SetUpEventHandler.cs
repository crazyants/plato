using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Entities.Handlers
{
    public class SetUpEventHandler : ISetUpEventHandler
    {

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
