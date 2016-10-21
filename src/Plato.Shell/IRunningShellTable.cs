using Plato.Shell.Models;
using System.Collections.Generic;

namespace Plato.Shell
{
    public interface IRunningShellTable
    {

        void Add(ShellSettings settings);

        void Remove(ShellSettings settings);

        ShellSettings Match(string host, string appRelativeCurrentExecutionFilePath);

        IDictionary<string, ShellSettings> ShellsByHostAndPrefix { get; }

    }

}
