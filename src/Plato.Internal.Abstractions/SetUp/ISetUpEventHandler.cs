using System;
using System.Threading.Tasks;

namespace Plato.Internal.Abstractions.SetUp
{
    public interface ISetUpEventHandler
    {

        string ModuleId { get; }

        Task SetUp(
            SetUpContext context,
            Action<string, string> reportError
        );

    }
}
