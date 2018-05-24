using System;
using System.Threading.Tasks;

namespace Plato.Abstractions.SetUp
{
    public interface ISetUpEventHandler
    {

        Task SetUp(
            SetUpContext context,
            Action<string, string> reportError
        );

    }
}
