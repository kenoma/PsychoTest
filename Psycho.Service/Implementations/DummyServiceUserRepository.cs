using Psycho.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Service.Implementations
{
    class DummyServiceUserRepository : IServiceUserRepository
    {
        public bool CheckUser(string login, string password)
        {
            return true;
        }
    }
}
