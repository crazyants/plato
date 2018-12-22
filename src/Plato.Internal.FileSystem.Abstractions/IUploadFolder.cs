using System.IO;
using System.Threading.Tasks;

namespace Plato.Internal.FileSystem.Abstractions
{
    public interface IUploadFolder
    {

        Task<string> SaveUniqueFileAsync(Stream stream, string fileName, string path);

    }
}
