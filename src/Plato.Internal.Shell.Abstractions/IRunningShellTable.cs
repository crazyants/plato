using System.Collections.Generic;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Shell.Abstractions
{
    public interface IRunningShellTable
    {

        void Add(IShellSettings settings);

        void Remove(IShellSettings settings);

        IShellSettings Match(string host, string appRelativeCurrentExecutionFilePath);

        IDictionary<string, IShellSettings> ShellsByHostAndPrefix { get; }

    }

}
