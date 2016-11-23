using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Stores.Files
{
    public interface IFileStore
    {

        Task<string> GetFileAsync(string path);

        Task<byte[]> GetFileBytesAsync(string path);

        string Combine(params string[] paths);

    }
}
