using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Implementations
{
    interface IProxyProvider
    {
        IWebProxy GetProxy();
        void PullBack(IWebProxy proxy, double speed);
    }
}
