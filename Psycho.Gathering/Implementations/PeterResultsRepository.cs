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
    public class PeterResultsRepository : SqLiteBaseRepository, IPeterResultsRepository
    {
        public PeterResultsRepository(string dbFile) : base(dbFile)
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
                     @"CREATE TABLE IF NOT EXISTS petergroup
                      (
                         Id                                  integer primary key AUTOINCREMENT,
                         VkontakteUserId                     integer not null,
                         user_type integer not null,
                         age TEXT,
                         gender integer not null
                       );");
            }
        }


        public void CleanAll()
        {
            using (var cn = DbConnection())
            {
                cn.Open();
                cn.Query(@"DROP TABLE IF EXISTS petergroup");
            }
            CreateTable();
        }

        public void Insert(IEnumerable<PeterModel> data)
        {
            using (var cn = DbConnection())
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    foreach (var item in data)
                    {
                        cn.Query(@"INSERT INTO petergroup (VkontakteUserId, user_type,age,gender) 
                                          VALUES (@user_id, @user_type, @age, @gender);", item);
                    }
                    trans.Commit();
                }
            }
        }
    }
}
