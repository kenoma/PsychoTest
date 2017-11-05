using Psycho.Gathering.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Psycho.Gathering.Models;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using Psycho.Gathering.Models.Repo;
using System.Diagnostics;
using Dapper;
using SevenZip.SDK;
using Serilog;
using ProtoBuf;
using LZ4;

namespace Psycho.Gathering.Implementations
{
    public class WallPostRepository : SqLiteBaseRepository, IWallPostRepository
    {
        private string _pathToStorage;
        private ILogger _log;
        private readonly Timer _endTransactionTimer;
        private readonly ConcurrentQueue<WallPostDataChunk> _saveQueue = new ConcurrentQueue<WallPostDataChunk>();

        public WallPostRepository(string pathToStorage, ILogger log)
            : base(pathToStorage)
        {
            _pathToStorage = pathToStorage;
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            _log = log;
            _endTransactionTimer = new Timer(RenewTransactionCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        internal void RenewTransactionCallback(object state)
        {
            try
            {
                if (_saveQueue.IsEmpty)
                    return;

                var sw = Stopwatch.StartNew();
                using (var cn = DbConnection())
                {
                    cn.Open();
                    using (var trans = cn.BeginTransaction())
                    {
                        while (_saveQueue.TryDequeue(out WallPostDataChunk chunk))
                            cn.Query(@"INSERT INTO WallPosts (GroupId, CompressedData) VALUES (@GroupId, @CompressedData);", chunk);
                        trans.Commit();
                    }
                }
                _log?.Verbose($"Commitinf transaction takes {sw.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }
        }

        public IReadOnlyCollection<byte[]> RangeRawSelect(int skip, int take)
        {
            var retval = new List<byte[]>();
            try
            {
                using (var cnn = DbConnection())
                {
                    cnn.Open();
                    var chunks = cnn.Query<WallPostDataChunk>(
                        $"SELECT * FROM WallPosts WHERE Id > {skip} ORDER BY Id LIMIT {take}").ToArray();
                    foreach (var chunk in chunks)
                        retval.Add(chunk.CompressedData);
                }
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }

            return retval;
        }

        public IReadOnlyCollection<WallResponse> RangeSelect(int skip, int take)
        {
            var retval = new List<WallResponse>();
            try
            {
                using (var cnn = DbConnection())
                {
                    cnn.Open();
                    var chunks = cnn.Query<WallPostDataChunk>(
                        $"SELECT * FROM WallPosts WHERE Id > {skip} ORDER BY Id LIMIT {take}").ToArray();
                    foreach (var chunk in chunks)
                        retval.Add(DecompressFileLZMA(chunk.CompressedData));
                }
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }

            return retval;
        }

        public bool SaveWallpost(WallResponse wallData)
        {
            try
            {
                if (wallData == null)
                    return false;
                var chunk = new WallPostDataChunk
                {

                    GroupId = wallData.GroupId,
                    CompressedData = CompressFile(wallData)
                };
                _saveQueue.Enqueue(chunk);

                return true;
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }
            return false;
        }

        private static byte[] CompressFile(WallResponse inStream)
        {
            using (var tg = new MemoryStream())
            //using (var ms = new LZ4Stream(tg, LZ4StreamMode.Compress))
            {
                Serializer.Serialize(tg, inStream);
                tg.Position = 0;
                return LZ4Codec.Wrap(tg.ToArray());
            }
            //Int32 dictionary = 1 << 23;
            //Int32 posStateBits = 2;
            //Int32 litContextBits = 3; // for normal files
            //// UInt32 litContextBits = 0; // for 32-bit data
            //Int32 litPosBits = 0;
            //// UInt32 litPosBits = 2; // for 32-bit data
            //Int32 algorithm = 2;
            //Int32 numFastBytes = 128;

            //string mf = "bt4";
            //bool eos = true;
            //bool stdInMode = false;


            //CoderPropID[] propIDs =  {
            //    CoderPropID.DictionarySize,
            //    CoderPropID.PosStateBits,
            //    CoderPropID.LitContextBits,
            //    CoderPropID.LitPosBits,
            //    CoderPropID.Algorithm,
            //    CoderPropID.NumFastBytes,
            //    CoderPropID.MatchFinder,
            //    CoderPropID.EndMarker
            //};

            //object[] properties = {
            //    (Int32)(dictionary),
            //    (Int32)(posStateBits),
            //    (Int32)(litContextBits),
            //    (Int32)(litPosBits),
            //    (Int32)(algorithm),
            //    (Int32)(numFastBytes),
            //    mf,
            //    eos
            //};

            //using (var outStream = new MemoryStream())
            //{
            //    var encoder = new SevenZip.SDK.Compress.LZMA.Encoder();
            //    encoder.SetCoderProperties(propIDs, properties);
            //    encoder.WriteCoderProperties(outStream);
            //    Int64 fileSize;
            //    if (eos || stdInMode)
            //        fileSize = -1;
            //    else
            //        fileSize = inStream.Length;
            //    for (int i = 0; i < 8; i++)
            //        outStream.WriteByte((Byte)(fileSize >> (8 * i)));
            //    encoder.Code(inStream, outStream, -1, -1, null);
            //    return outStream.ToArray();
            //}
        }

        private static WallResponse DecompressFileLZMA(byte[] inFile)
        {
            using (var src = new MemoryStream(LZ4Codec.Unwrap(inFile)))
            {
                src.Position = 0;
                return Serializer.Deserialize<WallResponse>(src);
            }
            //    using (var input = new MemoryStream(inFile))
            //    using (var output = new MemoryStream())
            //    {
            //        var decoder = new SevenZip.SDK.Compress.LZMA.Decoder();

            //        byte[] properties = new byte[5];
            //        if (input.Read(properties, 0, 5) != 5)
            //            throw (new Exception("input .lzma is too short"));
            //        decoder.SetDecoderProperties(properties);

            //        long outSize = 0;
            //        for (int i = 0; i < 8; i++)
            //        {
            //            int v = input.ReadByte();
            //            if (v < 0)
            //                throw (new Exception("Can't Read 1"));
            //            outSize |= ((long)(byte)v) << (8 * i);
            //        }
            //        long compressedSize = input.Length - input.Position;

            //        decoder.Code(input, output, compressedSize, outSize, null);
            //        output.Position = 0;
            //        return ProtoBuf.Serializer.Deserialize<WallResponse>(output);
            //    }
        }

        private void CreateDatabase()
        {
            using (var cnn = DbConnection())
            {
                cnn.Open();
                cnn.Execute(
                    @"create table WallPosts
                      (
                         Id                                  integer primary key AUTOINCREMENT,
                         GroupId                             integer not null,
                         CompressedData                      BLOB
                      );
                    PRAGMA main.page_size = 4096;
                    PRAGMA main.cache_size=10000;
                    PRAGMA main.locking_mode=EXCLUSIVE;
                    PRAGMA main.journal_mode=WAL;
                    PRAGMA main.temp_store = MEMORY;");
            }
        }
    }
}
