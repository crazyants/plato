using System.Collections.Generic;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Shell.Abstractions
{
    public interface IRunningShellTable
    {

        void Add(ShellSettings settings);

        void Remove(ShellSettings settings);

        ShellSettings Match(string host, string appRelativeCurrentExecutionFilePath);

        IDictionary<string, ShellSettings> ShellsByHostAndPrefix { get; }

    }

}
