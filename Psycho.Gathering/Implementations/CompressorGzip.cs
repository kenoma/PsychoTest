using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Psycho.Gathering.Models;
using System.IO;
using ProtoBuf;
using SevenZip.SDK;
using System.IO.Compression;

namespace Psycho.Gathering.Implementations
{
    public class CompressorGzip : ICompressor
    {
        public byte[] CompressFile(UserGet chunk)
        {
            using (var outStream = new MemoryStream())
            {
                using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
                using (var mStream = new MemoryStream())
                {
                    Serializer.Serialize(mStream, chunk);
                    mStream.Position = 0;
                    mStream.CopyTo(tinyStream);
                }

                return outStream.ToArray();
            }
        }

        public UserGet Decompress(byte[] inFile)
        {
            using (var inStream = new MemoryStream(inFile))
            using (var bigStream = new GZipStream(inStream, CompressionMode.Decompress))
            using (var bigStreamOut = new MemoryStream())
            {
                bigStream.CopyTo(bigStreamOut);
                bigStreamOut.Position = 0;
                return Serializer.Deserialize<UserGet>(bigStreamOut);
            }
        }
    }
}
