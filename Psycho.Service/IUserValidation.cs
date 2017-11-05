using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Service
{
    internal interface IUserValidation
    {
        bool ValidateToken(string token);
        string Auth(string login, string password);
    }
}
