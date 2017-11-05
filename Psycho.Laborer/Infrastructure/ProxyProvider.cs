using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Infrastructure
{
    class ProxyProvider : IProxyProvider
    {
        private readonly ILogger _log;
        private IWebProxy[] _proxyAddress;
        private string _fineproxyUrl;

        public ProxyProvider(string fineproxyUrl, ILogger log)
        {
            _log = log;
            _log.Information("Get proxy list...");
            _fineproxyUrl = fineproxyUrl;
        }

        public IWebProxy[] RetrieveProxyList()
        {
            if (_proxyAddress?.Any() ?? false)
            {
                return _proxyAddress;
            }

            var proxies = Request(_fineproxyUrl);
            _proxyAddress = proxies.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(z => new WebProxy(z)).ToArray();
            return _proxyAddress;
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
