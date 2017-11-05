using Psycho.Common.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Repository.Local
{
    public interface IRepository<T> where T : ILocalAggregateRoot
    {
        void Add(IDbConnection cn,T item);
        void RemoveById(IDbConnection cn, T item);
        T FindById(IDbConnection cn, long id);
        IEnumerable<T> FindAll(IDbConnection cn);
        void UpdateById(IDbConnection cn, T item);
    }
}
