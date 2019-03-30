using System.IO;
using System.Threading.Tasks;

namespace Plato.Internal.FileSystem.Abstractions
{
    /// <summary>
    /// Provides isolation for the uploads folder within Plato.
    /// </summary>
    public interface ISitesFolder
    {
  
        string RootPath { get; }

        Task<string> SaveUniqueFileAsync(Stream stream, string fileName, string path);

        Task<string> SaveFileAsync(Stream stream, string fileName, string path);

        Task CreateFileAsync(string path, Stream stream);

        bool DeleteFile(string fileName, string path);

        string Combine(params string[] paths);

    }

}
