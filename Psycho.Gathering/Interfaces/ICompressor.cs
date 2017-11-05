using Psycho.Gathering.Models;

namespace Psycho.Gathering.Implementations
{
    public interface ICompressor
    {
        byte[] CompressFile(UserGet chunk);
        UserGet Decompress(byte[] inFile);
    }
}