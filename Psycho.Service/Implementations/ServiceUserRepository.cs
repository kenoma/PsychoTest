using MongoDB.Driver;
using Psycho.Common.Domain.AdminArea;
using Psycho.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Service.Implementations
{
    class ServiceUserRepository : IServiceUserRepository
    {
        private IMongoDatabase _database;

        public ServiceUserRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public bool CheckUser(string login, string password)
        {
            var users = _database.GetCollection<ServiceUser>(nameof(ServiceUser));
            var count = users.Count(z => z.Username == login && z.Password == password);
            return count != 0;
        }
    }
}
