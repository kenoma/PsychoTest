using Flurl;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Psycho.Laborer.Infrastructure
{
    class UnitRequestor
    {
        public string VkKey { get; set; }
        public string UserAgent { get; set; }
        public IWebProxy Proxy { get; set; }
        public int RequestCounter { get; set; }
        public DateTime LastRequest { get; private set; } = DateTime.Now;
        public bool IsSpoiled { get; private set; }
        private readonly JsonSerializerSettings _jsonSettings =new JsonSerializerSettings { Error = HandleDeserializationError };
        private readonly object _locker = new object();

        public T GetRequest<T>(string method,object queryParams)
        {
            try
            {
                var requestText = "https://api.vk.com/method"
                    .AppendPathSegment(method)
                    .SetQueryParams(queryParams)
                    .SetQueryParam("access_token", VkKey)
                    .SetQueryParam("v", "5.67")
                    .ToString(false);

                var resp = Request(requestText);
                if (string.IsNullOrEmpty(resp))
                    return default(T);

                return JsonConvert.DeserializeObject<T>(resp, _jsonSettings);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return default(T);
        }

        private string Request(string url)
        {
            lock (_locker)
            {
                if ((DateTime.Now - LastRequest).TotalMilliseconds < Config.Default.RequestIntervalMs)
                {
                    Thread.Sleep(Convert.ToInt32(Config.Default.RequestIntervalMs - (DateTime.Now - LastRequest).TotalMilliseconds));
                }

                LastRequest = DateTime.Now;
                RequestCounter++;
                string html = string.Empty;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = Proxy;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.UserAgent = UserAgent;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
                if (html.Contains("\"error_msg\":\"Too many requests per second\""))
                {
                    return string.Empty;
                }

                if (html.Contains("User authorization failed: invalid session."))
                    IsSpoiled = true;

                return html;
            }
        }

        static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
            Log.Error(currentError);
        }
    }
}
