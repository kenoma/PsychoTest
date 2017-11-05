using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Infrastructure
{
    interface IProxyProvider
    {
        IWebProxy[] RetrieveProxyList();
    }
}
