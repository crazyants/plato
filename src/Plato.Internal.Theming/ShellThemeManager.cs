using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Models;

namespace Plato.Internal.Theming
{

    public interface IShellThemeManager

    {
        Task<ICommandResult<ThemeDescriptor>> CreateAsync(string sourceTheme, string target);
        }

    public class ShellThemeManager : IShellThemeManager
    {

        private readonly IUploadFolder _uploadFolder;

        public ShellThemeManager(
            IUploadFolder uploadFolder)
        {
            _uploadFolder = uploadFolder;
        }

        public Task<ICommandResult<ThemeDescriptor>> CreateAsync(string sourceTheme, string target)
        {
            throw new NotImplementedException();
        }
    }
}
