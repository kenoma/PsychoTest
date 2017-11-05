using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Implementations
{
    class ProxyProvider : IProxyProvider
    {
        private readonly ILogger _log;
        private readonly ConcurrentQueue<IWebProxy> _proxyAddress;
        private readonly Random rnd = new Random(Environment.TickCount);
        
        public ProxyProvider(string fineproxyUrl, ILogger log)
        {
            _log = log;
            _log.Information("Get proxy list...");
            var proxies = Request(fineproxyUrl);
            _proxyAddress = new ConcurrentQueue<IWebProxy>(proxies.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(z => new WebProxy(z)));
        }

        public IWebProxy GetProxy()
        {
            if (_proxyAddress.IsEmpty)
                return null;
            if (_proxyAddress.TryDequeue(out IWebProxy proxy))
                return proxy;

            return null;
        }

        public void PullBack(IWebProxy proxy, double speed)
        {
            _proxyAddress.Enqueue(proxy);
        }

        private string Request(string url)
        {
            var html = string.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.85 Safari/537.36";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, url);
            }
            return html;
        }
    }
}
