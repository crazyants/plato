using System.Threading.Tasks;

namespace Plato.Internal.Stores.Abstractions.Files
{
    public interface IFileStore
    {

        Task<string> GetFileAsync(string path);

        Task<byte[]> GetFileBytesAsync(string path);

        string Combine(params string[] paths);

    }
}
