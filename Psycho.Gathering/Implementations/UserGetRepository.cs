using Psycho.Gathering.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Psycho.Gathering.Models;
using System.IO;
using SevenZip.SDK;
using Dapper;
using System.Threading;
using System.Collections.Concurrent;
using Psycho.Gathering.Models.Repo;
using System.Diagnostics;
using ProtoBuf;
using LZ4;
using Serilog;

namespace Psycho.Gathering.Implementations
{
    public class UserGetRepository : SqLiteBaseRepository, IUserGetRepository
    {
        private readonly string _pathToDb;
        private readonly ILogger _log;
        private ICompressor _compressor;
        private readonly object _locker = new object();

        public UserGetRepository(string pathToDb, ILogger log, ICompressor compressor)
            : base(pathToDb)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }
            _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            Serializer.PrepareSerializer<UserGet>();
        }


        internal void PushDataToDataBase(IList<DataChunk> chunkList)
        {
            try
            {
                if ((chunkList?.Count() ?? 0) == 0)
                    return;
                _log?.Verbose($"Saving {chunkList.Count()} records");

                using (var cn = DbConnection())
                {
                    cn.Open();
                    using (var trans = cn.BeginTransaction())
                    {
                        foreach (var chunk in chunkList)
                            cn.Query(@"INSERT INTO UserGets (VkontakteUserId, Timestamp, CompressedUserGet) VALUES (@VkontakteUserId, @Timestamp, @CompressedUserGet);", chunk);

                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }
        }

        public bool SaveUser(UserGet userData, DateTime timestamp)
        {
            try
            {
                lock (_locker)
                {
                    var chunks = new List<DataChunk>();
                    var chunk = new DataChunk
                    {
                        Timestamp = timestamp,
                        VkontakteUserId = userData.id,
                        CompressedUserGet = _compressor.CompressFile(userData)
                    };

                    chunks.Add(chunk);
                    PushDataToDataBase(chunks);
                }
                return true;
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }

            return false;
        }



        public IReadOnlyCollection<UserGet> RangeSelect(int skip, int take)
        {
            var retval = new List<UserGet>();
            try
            {
                using (var cnn = DbConnection())
                {
                    cnn.Open();
                    var chunks = cnn.Query<DataChunk>(
                        $"SELECT * FROM UserGets LIMIT {take} OFFSET {skip}").ToArray();
                    foreach (var chunk in chunks)
                        retval.Add(_compressor.Decompress(chunk.CompressedUserGet));
                }
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }

            return retval;
        }

        public IReadOnlyCollection<Tuple<int, byte[]>> RangeRawSelect(int skip, int take)
        {
            var retval = new List<Tuple<int, byte[]>>();
            try
            {
                using (var cnn = DbConnection())
                {
                    cnn.Open();
                    var chunks = cnn.Query<DataChunk>(
                        $"SELECT * FROM UserGets WHERE Id > {skip} ORDER BY Id LIMIT {take}").ToArray();
                    foreach (var chunk in chunks)
                        retval.Add(new Tuple<int, byte[]>(chunk.VkontakteUserId, chunk.CompressedUserGet));
                }
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }

            return retval;
        }

        public IReadOnlyList<UserGet> GetUser(int id)
        {
            var retval = new List<UserGet>();
            try
            {
                using (var cnn = DbConnection())
                {
                    cnn.Open();
                    var chunks = cnn.Query<DataChunk>(
                        @"SELECT Id, VkontakteUserId, Timestamp, CompressedUserGet
                    FROM UserGets
                    WHERE VkontakteUserId = @id", new { id }).ToArray();
                    foreach (var chunk in chunks)
                    {
                        retval.Add(_compressor.Decompress(chunk.CompressedUserGet));
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }

            return retval;
        }

        private void CreateDatabase()
        {
            using (var cnn = DbConnection())
            {
                cnn.Open();
                cnn.Execute(
                    @"create table UserGets
                      (
                         Id                                  integer primary key AUTOINCREMENT,
                         VkontakteUserId                     integer not null,
                         Timestamp                           datetime not null,
                         CompressedUserGet                   BLOB
                      );
                    PRAGMA main.page_size = 4096;
                    PRAGMA main.cache_size=10000;
                    PRAGMA main.locking_mode=EXCLUSIVE;
                    PRAGMA main.journal_mode=WAL;
                    PRAGMA main.temp_store = MEMORY;");
            }
        }

        public IReadOnlyCollection<UserGetMetaDTO> ListRecords()
        {
            try
            {
                using (var cnn = DbConnection())
                {
                    cnn.Open();
                    return cnn.Query<UserGetMetaDTO>(@"SELECT VkontakteUserId, Timestamp FROM UserGets").ToArray();

                }
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }

            return null;
        }

        public IReadOnlyCollection<int> GetUserIds()
        {
            try
            {
                using (var cnn = DbConnection())
                {
                    cnn.Open();
                    var chunks = cnn.Query<int>(@"SELECT Id FROM UserGets").ToArray();
                    return chunks;
                }
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }

            return new int[0];
        }

        public IReadOnlyCollection<int> GetUserVkIds()
        {
            try
            {
                using (var cnn = DbConnection())
                {
                    cnn.Open();
                    var chunks = cnn.Query<int>(@"SELECT VkontakteUserId FROM UserGets").ToArray();
                    return chunks;
                }
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }

            return new int[0];
        }

        public void SaveUsers(IList<UserGet> users, DateTime timestamp)
        {
            lock (_locker)
            {
                var chunks = new List<DataChunk>();
                //Parallel.ForEach(users, user =>
                foreach (var user in users)
                {
                    var chunk = new DataChunk
                    {
                        Timestamp = timestamp,
                        VkontakteUserId = user.id,
                        CompressedUserGet = _compressor.CompressFile(user)
                    };

                    chunks.Add(chunk);

                }//);
                PushDataToDataBase(chunks);
            }
        }

        public IReadOnlyList<UserGet> GetUsers(int[] id)
        {
            var retval = new List<UserGet>();
            try
            {
                using (var cnn = DbConnection())
                {
                    cnn.Open();
                    var chunks = cnn.Query<DataChunk>(
                        @"SELECT Id, VkontakteUserId, Timestamp, CompressedUserGet
                    FROM UserGets
                    WHERE VkontakteUserId in @id", new { id }).ToArray();
                    foreach (var chunk in chunks)
                    {
                        retval.Add(_compressor.Decompress(chunk.CompressedUserGet));
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }

            return retval;
        }

        public bool SaveRawUsers(IEnumerable<Tuple<int, byte[]>> userData, DateTime timestamp)
        {
            using (var cn = DbConnection())
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    foreach (var chunk in userData)
                        cn.Query(@"INSERT INTO UserGets (VkontakteUserId, Timestamp, CompressedUserGet) VALUES (@VkontakteUserId, @Timestamp, @CompressedUserGet);",
                            new
                            {
                                VkontakteUserId = chunk.Item1,
                                Timestamp = timestamp,
                                CompressedUserGet = chunk.Item2
                            });

                    trans.Commit();
                }
            }
            return true;
        }

        public void DeleteUsers(IEnumerable<int> vkIds)
        {
            try
            {
                using (var cn = DbConnection())
                {
                    cn.Open();
                    _log.Information("Create temp table");
                    cn.Query("CREATE TEMP TABLE to_delete(VkontakteUserId integer not null);");
                    using (var trans = cn.BeginTransaction())
                    {
                        foreach (var id in vkIds)
                            cn.Query(@"INSERT INTO to_delete (VkontakteUserId) VALUES (@VkontakteUserId);", new { VkontakteUserId = id });
                        trans.Commit();
                    }
                    _log.Information("Perform deletion");
                    cn.Query(@"DELETE FROM UserGets WHERE VkontakteUserId in to_delete;");
                }

                _log.Information("Performing clean up");
                using (var cn = DbConnection())
                {
                    cn.Open();
                    cn.Query(@"VACUUM;");
                }
                _log.Information("Deletion done");
            }
            catch (Exception ex)
            {
                _log?.Error(ex, ex.Message);
            }
        }
    }
}
