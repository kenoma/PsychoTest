using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Repository
{
    public interface IServiceUserRepository
    {
        bool CheckUser(string login, string password);
    }
}
