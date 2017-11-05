using Dapper;
using Psycho.Gathering.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Implementations
{
    public class FifthResultRepository : SqLiteBaseRepository, IFifthResultRepository
    {
        public FifthResultRepository(string dbFile) 
            : base(dbFile)
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
                    @"CREATE TABLE IF NOT EXISTS fifth
                      (
                         Id                                  integer primary key AUTOINCREMENT,
                         VkontakteUserId                     integer not null,
                         Neirotism FLOAT not null,
                         OpenToExpirience FLOAT not null,
                         SelfControl FLOAT not null,
                         Agreement FLOAT not null,
                         Extraversion FLOAT not null
                      );");
            }
        }

        public void CleanAll()
        {
            using (var cn = DbConnection())
            {
                cn.Open();
                    cn.Query(@"DROP TABLE IF EXISTS fifth");   
            }
            CreateTable();
        }

        public void Insert(int vkId, float[] results)
        {
            using (var cn = DbConnection())
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    cn.Query(@"INSERT INTO fifth (VkontakteUserId, Neirotism, OpenToExpirience, SelfControl, Agreement, Extraversion) 
                                          VALUES (@Vk, @R1, @R2, @R3, @R4, @R5);", new { Vk = vkId, R1 = results[0], R2 = results[1], R3 = results[2], R4 = results[3], R5 = results[4] });

                    trans.Commit();
                }
            }
        }

        public void Insert(int[] vkIds, float[][] aresults)
        {
            using (var cn = DbConnection())
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    for (int pos = 0; pos < vkIds.Length; pos++)
                    {
                        cn.Query(@"INSERT INTO fifth (VkontakteUserId, Neirotism, OpenToExpirience, SelfControl, Agreement, Extraversion) 
                                          VALUES (@Vk, @R1, @R2, @R3, @R4, @R5);", new { Vk = vkIds[pos], R1 = aresults[pos][0], R2 = aresults[pos][1], R3 = aresults[pos][2], R4 = aresults[pos][3], R5 = aresults[pos][4] });
                    }
                    trans.Commit();
                }
            }
        }

       
    }
}
