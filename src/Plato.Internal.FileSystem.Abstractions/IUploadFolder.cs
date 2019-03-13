using System.IO;
using System.Threading.Tasks;

namespace Plato.Internal.FileSystem.Abstractions
{
    public interface IUploadFolder
    {
  
        string InternalRootPath { get; }

        Task<string> SaveUniqueFileAsync(Stream stream, string fileName, string path);

        Task<string> SaveFileAsync(Stream stream, string fileName, string path);

        Task CreateFileAsync(string path, Stream stream);

        bool DeleteFile(string fileName, string path);

    }
}
