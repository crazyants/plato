using System;
using System.Threading.Tasks;

namespace Plato.Internal.Abstractions.SetUp
{
    public interface ISetUpEventHandler
    {

        Task SetUp(
            SetUpContext context,
            Action<string, string> reportError
        );

    }
}
