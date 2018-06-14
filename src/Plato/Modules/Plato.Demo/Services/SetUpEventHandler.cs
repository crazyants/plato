using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Demo.Services
{
    public class SetUpEventHandler : ISetUpEventHandler
    {

        private readonly ISchemaBuilder _schemaBuilder;
    
        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }

        public Task SetUp(
            SetUpContext context,
            Action<string, string> reportError)
        {
            return Task.CompletedTask;

        }

    }

}
