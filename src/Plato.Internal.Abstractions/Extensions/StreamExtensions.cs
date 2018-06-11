using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace Plato.Internal.Abstractions.Extensions
{
    public static class StreamExtensions
    {

        public static string StreamToString(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }


        public static byte[] StreamToByteArray(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            var fileData = new byte[(int)stream.Length - 1 + 1];
            stream.Read(fileData, 0, fileData.Length);
            return fileData;
        }


    }
}
