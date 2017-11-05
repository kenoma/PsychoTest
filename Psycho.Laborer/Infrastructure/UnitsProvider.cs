using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Infrastructure
{
    class UnitsProvider
    {
        private List<UnitRequestor> _pool = new List<UnitRequestor>();
        private Random _rnd = new Random(Environment.TickCount);

        public UnitsProvider(IList<string> vkKeys, IProxyProvider proxyProvider, IList<string> userAgents)
        {
            if (!vkKeys.Any() || !userAgents.Any())
                throw new Exception("Inproper initialization.");

            _pool.AddRange(vkKeys.Zip(proxyProvider.RetrieveProxyList(), (x, y) => new UnitRequestor { Proxy = y, VkKey = x, UserAgent = userAgents[_rnd.Next(userAgents.Count)] }));
        }


        public UnitRequestor GetRequestor()
        {
            var clean = _pool.Where(z => !z.IsSpoiled);
            if (!clean.Any())
                throw new Exception("Remains no clear requestors");

            return clean.OrderBy(z => -(DateTime.Now - z.LastRequest).TotalSeconds).ThenBy(z => _rnd.NextDouble()).First();
        }
    }
}
