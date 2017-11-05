using Psycho.Gathering.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Psycho.Gathering.Model;
using Dapper;
using System.IO;

namespace Psycho.Gathering.Implementations
{
    public class AntiBotRepository : SqLiteBaseRepository, IAntiBotRepository
    {
        readonly object _locker = new object();

        public AntiBotRepository(string dbFile) : base(dbFile)
        {
            if (!File.Exists(DbFile))
            {
                throw new Exception("Should be called only on existing repo");
            }
            CreateTable();
        }

        private void CreateTable()
        {
            using (var cnn = DbConnection())
            {
                cnn.Open();
                cnn.Execute(
                     @"CREATE TABLE IF NOT EXISTS antibot
                      (
                         Id                                  integer primary key AUTOINCREMENT,
                         VkontakteUserId                     integer not null,
                         isBot                               integer not null
                       );");
            }
        }

        public void SaveUsers(IEnumerable<Tuple<int, bool>> isBotData)
        {
            lock (_locker)
            {
                using (var cn = DbConnection())
                {
                    cn.Open();
                    using (var trans = cn.BeginTransaction())
                    {
                        foreach (var item in isBotData)
                        {
                            cn.Query(@"INSERT INTO antibot (VkontakteUserId, isBot) 
                                          VALUES (@user_id, @isBot);", new { user_id = item.Item1, isBot = item.Item2 });
                        }
                        trans.Commit();
                    }
                }
            }
        }

        public IReadOnlyCollection<int> GetUserVkIds()
        {
            try
            {
                using (var cnn = DbConnection())
                {
                    cnn.Open();
                    var chunks = cnn.Query<int>(@"SELECT VkontakteUserId FROM antibot").ToArray();
                    return chunks;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return new int[0];
        }
    }
}
