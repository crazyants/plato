using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Theming.Abstractions;

namespace Plato.Theming.Services
{
    
    public class SiteThemeFileManager : ISiteThemeFileManager
    {
        
        private readonly IThemeFileManager _themeFileManager;

        public SiteThemeFileManager(
            ISiteThemeLoader siteThemeLoader,
            IThemeFileManager themeFileManager)
        {
            _themeFileManager = themeFileManager;
            SetThemeLoader(siteThemeLoader);
        }

        public void SetThemeLoader(IThemeLoader loader)
        {
            _themeFileManager.SetThemeLoader(loader);
        }

        public IEnumerable<IThemeFile> GetFiles(string themeId)
        {
            return _themeFileManager.GetFiles(themeId);
        }

        public IEnumerable<IThemeFile> GetFiles(string themeId, string relativePath)
        {
            return _themeFileManager.GetFiles(themeId, relativePath);
        }

        public IThemeFile GetFile(string themeId, string relativePath)
        {
            return _themeFileManager.GetFile(themeId, relativePath);
        }

        public IEnumerable<IThemeFile> GetParents(string themeId, string relativePath)
        {
            return _themeFileManager.GetParents(themeId, relativePath);
        }

        public async Task<string> ReadFileAsync(IThemeFile themeFile)
        {
            return await _themeFileManager.ReadFileAsync(themeFile);
        }

        public async Task SaveFileAsync(IThemeFile themeFile, string contents)
        {
            await _themeFileManager.SaveFileAsync(themeFile, contents);
        }
    }

}
