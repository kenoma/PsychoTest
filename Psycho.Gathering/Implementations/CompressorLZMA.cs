using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Psycho.Gathering.Models;
using System.IO;
using ProtoBuf;
using SevenZip.SDK;

namespace Psycho.Gathering.Implementations
{
    public class CompressorLZMA : ICompressor
    {
        public byte[] CompressFile(UserGet chunk)
        {
            Int32 dictionary = 1 << 23;
            Int32 posStateBits = 2;
            Int32 litContextBits = 3; // for normal files
            // UInt32 litContextBits = 0; // for 32-bit data
            Int32 litPosBits = 0;
            // UInt32 litPosBits = 2; // for 32-bit data
            Int32 algorithm = 2;
            Int32 numFastBytes = 128;

            string mf = "bt4";
            bool eos = true;
            bool stdInMode = false;

            CoderPropID[] propIDs =  {
                CoderPropID.DictionarySize,
                CoderPropID.PosStateBits,
                CoderPropID.LitContextBits,
                CoderPropID.LitPosBits,
                CoderPropID.Algorithm,
                CoderPropID.NumFastBytes,
                CoderPropID.MatchFinder,
                CoderPropID.EndMarker
            };

            object[] properties = {
                (Int32)(dictionary),
                (Int32)(posStateBits),
                (Int32)(litContextBits),
                (Int32)(litPosBits),
                (Int32)(algorithm),
                (Int32)(numFastBytes),
                mf,
                eos
            };

            using (var tg = new MemoryStream())
            using (var outStream = new MemoryStream())
            {
                Serializer.Serialize(tg, chunk);
                tg.Position = 0;
                var encoder = new SevenZip.SDK.Compress.LZMA.Encoder();
                encoder.SetCoderProperties(propIDs, properties);
                encoder.WriteCoderProperties(outStream);
                Int64 fileSize;
                if (eos || stdInMode)
                    fileSize = -1;
                else
                    fileSize = tg.Length;
                for (int i = 0; i < 8; i++)
                    outStream.WriteByte((Byte)(fileSize >> (8 * i)));
                encoder.Code(tg, outStream, -1, -1, null);
                return outStream.ToArray();
            }
        }

        public UserGet Decompress(byte[] inFile)
        {
            using (var input = new MemoryStream(inFile))
            using (var output = new MemoryStream())
            {
                var decoder = new SevenZip.SDK.Compress.LZMA.Decoder();

                byte[] properties = new byte[5];
                if (input.Read(properties, 0, 5) != 5)
                    throw (new Exception("input .lzma is too short"));
                decoder.SetDecoderProperties(properties);

                long outSize = 0;
                for (int i = 0; i < 8; i++)
                {
                    int v = input.ReadByte();
                    if (v < 0)
                        throw (new Exception("Can't Read 1"));
                    outSize |= ((long)(byte)v) << (8 * i);
                }
                long compressedSize = input.Length - input.Position;

                decoder.Code(input, output, compressedSize, outSize, null);
                output.Position = 0;
                return Serializer.Deserialize<UserGet>(output);
            }
        }
    }
}
