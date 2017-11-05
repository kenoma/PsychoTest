using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Service.Interfaces
{
    interface ISocialNetworkGrabber<T>
    {
        T GetUserData(string access_token);
    }
}
