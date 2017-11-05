
using Dapper;
using Psycho.Laborer.Repo.Script;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Repository.Local
{
    static class SqliteHelpers
    {

        public static T Insert<T>(this IDbConnection cnn, string tableName, dynamic param)
        {
            IEnumerable<T> result = SqlMapper.Query<T>(cnn, GetInsertQuery(tableName, param), param);
            return result.First();
        }

        public static string GetUpdateQuery(string tableName, object item)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            var columns = props.Select(p => p.Name).ToArray();

            var parameters = columns.Select(name => name + "=@" + name).ToList();
            var par = parameters.Where(x => !x.Equals("Id=@Id")).ToList();

            return string.Format("UPDATE {0} SET {1} WHERE Id=@Id", tableName, string.Join(",", par));
        }


        public static string GetInsertQuery(string tableName, object item, params string[] ignored)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            var columns = props.Select(p => p.Name).Except(ignored).ToArray();

            return $"INSERT OR IGNORE INTO {tableName} ({string.Join(",", columns)}) VALUES (@{string.Join(",@", columns)});";
        }

        static public void EnsureCreated(string connectionString)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                conn.Execute(Queries.CreateDb);
            }
        }

        static public IDbConnection CreateAndOpenConnection(string connectionString)
        {
            var conn = new SQLiteConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}
