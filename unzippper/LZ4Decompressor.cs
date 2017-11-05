using System.Runtime.InteropServices;
using System.Text;
using LZ4;
using unzippper;
using System.Reflection;

namespace lz4csdecompressor
{
    public class LZ4Decompressor
    {
        [DllExport("DecompressLZ4ByteArray", CallingConvention = CallingConvention.Cdecl)]
        static string DecompressLZ4ByteArray(byte[] compressed)
        {
            return Encoding.UTF8.GetString(LZ4Codec.Unwrap(compressed), 0, compressed.Length);
        }
    }
}