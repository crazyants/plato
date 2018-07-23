using System;
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

            // Important: Ensure we reset the stream position
            stream.Position = 0;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
        
        public static byte[] StreamToByteArray(this Stream stream)
        {
            
            // Important: Ensure we reset the stream position
            stream.Position = 0;

            // Create a btye array buffer matching the stream size
            var buffer = new byte[stream.Length];

            // Read the bytes from the steam
            for (var i = 0; i < stream.Length; i++)
            {
                i += stream.Read(
                    buffer,
                    i,
                    Convert.ToInt32(stream.Length) - i);
            }
                
            return buffer;

        }

    }
}
