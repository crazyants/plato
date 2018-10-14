using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Demo.Handlers
{
    public class SetUpEventHandler : BaseSetUpEventHandler
    {

        private readonly ISchemaBuilder _schemaBuilder;
    
        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }

        public override Task SetUp(
            SetUpContext context,
            Action<string, string> reportError)
        {
            return Task.CompletedTask;
        }

    }

}
