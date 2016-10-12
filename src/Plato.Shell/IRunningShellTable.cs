using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Shell
{
    public interface IRunningShellTable
    {
        void Add(ShellSettings settings);
        void Remove(ShellSettings settings);
        ShellSettings Match(string host, string appRelativeCurrentExecutionFilePath);
    }
}
