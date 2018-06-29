using System;
using System.IO;
using System.Threading.Tasks;

namespace Plato.Internal.Abstractions.SetUp
{
    public abstract class BaseSetUpEventHandler : ISetUpEventHandler
    {

        public string ModuleId => Path.GetFileNameWithoutExtension(this.GetType().Assembly.ManifestModule.Name);

        public abstract Task SetUp(SetUpContext context, Action<string, string> reportError);

    }
}
