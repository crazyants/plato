using System.IO;
using System.Threading.Tasks;

namespace Plato.Internal.FileSystem.Abstractions
{
    public interface IUploadFolder
    {
  
        Task<string> SaveFileAsync(Stream stream, string fileName, string path);

        bool DeleteFile(string fileName, string path);

    }
}
