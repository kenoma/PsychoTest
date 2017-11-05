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
    public class DenisResultsRepository : SqLiteBaseRepository, IDenisResultsRepository
    {
        public DenisResultsRepository(string dbFile) : base(dbFile)
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
                     @"CREATE TABLE IF NOT EXISTS heuristics
                      (
                         Id                                  integer primary key AUTOINCREMENT,
                         VkontakteUserId                     integer not null,
                         gender TEXT,
                        age TEXT,
                        opp_count FLOAT not null,
                        pat_count_if_opp FLOAT not null,
                        stage_1 FLOAT not null,
                        stage_2 FLOAT not null,
                        stage_3 FLOAT not null,
                        stage_4 FLOAT not null,
                        stage_5 FLOAT not null,
                        stage_6 FLOAT not null
                       );");
            }
        }


        public void CleanAll()
        {
            using (var cn = DbConnection())
            {
                cn.Open();
                cn.Query(@"DROP TABLE IF EXISTS heuristics");
            }
            CreateTable();
        }

        public void Insert(IEnumerable<DenisModel> data)
        {
            using (var cn = DbConnection())
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    foreach (var item in data)
                    {
                        cn.Query(@"INSERT INTO heuristics (VkontakteUserId, gender, age, opp_count, pat_count_if_opp,stage_1,stage_2,stage_3,stage_4,stage_5, stage_6) 
                                          VALUES (@user_id, @gender, @age, @opp_count, @pat_count_if_opp,@stage_1,@stage_2,@stage_3,@stage_4,@stage_5, @stage_6);", item);
                    }
                    trans.Commit();
                }
            }
        }
    }
}
