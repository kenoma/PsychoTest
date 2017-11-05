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
    public class CompressorProto : ICompressor
    {
        public byte[] CompressFile(UserGet chunk)
        {
            using (var mStream = new MemoryStream())
            {
                Serializer.Serialize(mStream, chunk);
                return mStream.ToArray();
            }
        }

        public UserGet Decompress(byte[] inFile)
        {
            using (var bigStreamOut = new MemoryStream(inFile))
            {
                return Serializer.Deserialize<UserGet>(bigStreamOut);
            }
        }
    }
}
