using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Implementations
{
    class DummyProxyProvider : IProxyProvider
    {
        public IWebProxy GetProxy()
        {
            return null;
        }

        public void PullBack(IWebProxy proxy, double speed)
        {
        }
    }
}
