using Dapper;
using Psycho.Common.Repository.Local;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo
{
    public abstract class Repository<T> : IRepository<T> where T : ILocalAggregateRoot
    {
        protected readonly string _tableName;
        
        protected Repository(string tableName)
        {
            _tableName = tableName;
        }

        internal virtual object Mapping(T item)
        {
            return item;
        }

        public virtual void Add(IDbConnection cn, T item)
        {
            var parameters = Mapping(item);
            cn.Execute(SqliteHelpers.GetInsertQuery(_tableName, parameters), parameters);
        }

        public virtual void RemoveById(IDbConnection cn, T item)
        {
            cn.Execute($"DELETE FROM {_tableName} WHERE Id=@Id", new { Id = item.id });
        }

        public virtual T FindById(IDbConnection cn, long id)
        {
            T item = default(T);

            item = cn.QuerySingleOrDefault<T>($"SELECT * FROM {_tableName} WHERE Id=@Id", new { Id = id });

            return item;
        }

        public virtual IEnumerable<T> FindAll(IDbConnection cn)
        {
            IEnumerable<T> items = null;
            items = cn.Query<T>($"SELECT * FROM {_tableName}");
            return items;
        }

        public void UpdateById(IDbConnection cn, T item)
        {
            var p = Mapping(item);
            cn.Execute(SqliteHelpers.GetUpdateQuery(_tableName, p), p);
        }
    }
}
