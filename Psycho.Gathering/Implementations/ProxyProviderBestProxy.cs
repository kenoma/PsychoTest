using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Implementations
{
    class ProxyProviderBestProxy : IProxyProvider
    {
        private readonly ILogger _log;
        private readonly List<string> _proxyAddress;
        private readonly Random rnd = new Random(Environment.TickCount);

        public ProxyProviderBestProxy(string bestproxyUrl, ILogger log)
        {
            _log = log;
            _log.Information("Get proxy list...");
            var proxies = Request(bestproxyUrl);
            _proxyAddress = proxies.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public IWebProxy GetProxy(int seed = -1)
        {
            if (_proxyAddress.Any())
            {
                var ele = _proxyAddress[0];
                _proxyAddress.Remove(ele);
                return new WebProxy(ele);
            }
            else
                return null;
            //if (seed >= 0 && seed < _proxyAddress.Length)
            //    return new WebProxy(_proxyAddress[seed]);
            //else
            //    return new WebProxy(_proxyAddress[rnd.Next(_proxyAddress.Length)]);
        }

        public IWebProxy GetProxy()
        {
            throw new NotImplementedException();
        }

        public void PullBack(IWebProxy proxy, double speed)
        {
            throw new NotImplementedException();
        }

        private string Request(string url)
        {
            string html = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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
                _log.Error(ex,ex.Message);
            }
            return html;
        }
    }
}
