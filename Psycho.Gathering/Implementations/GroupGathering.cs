using Newtonsoft.Json;
using Psycho.Gathering.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace Psycho.Gathering.Implementations
{
    internal class GroupGathering
    {
        private readonly ILogger _log;
        private readonly AutoResetEvent _holder = new AutoResetEvent(false);
        private int RequestsDelayMs = 1000;
        private readonly string _vkAccessToken = "";
        private readonly object _locker = new object();
        private readonly IProxyProvider _proxy;

        public bool IsSpoiled { get; private set; } = false;
        public int Id { get; set; }

        public GroupGathering(ILogger log, IProxyProvider proxy, string vkAccessToken)
        {
            _vkAccessToken = vkAccessToken ?? throw new ArgumentException(nameof(vkAccessToken));
            _log = log ?? throw new ArgumentException(nameof(log));
            _proxy = proxy;
        }

        public WallResponse FillWallInfo(int groupId)
        {
            var wall100 = Request($"https://api.vk.com/method/wall.get?v=5.52&access_token={_vkAccessToken}&owner_id=-{groupId}&extended=1&fields=city,country,place,description,wiki_page,members_count,counters,start_date,finish_date,can_post,can_see_all_posts,activity,status,contacts,links,fixed_post,verified,site,can_create_topic&offset=0&count=100");
            //var wall200 = Request($"https://api.vk.com/method/wall.get?v=5.52&access_token={_vkAccessToken}&owner_id=-{groupId}&extended=1&fields=city,country,place,description,wiki_page,members_count,counters,start_date,finish_date,can_post,can_see_all_posts,activity,status,contacts,links,fixed_post,verified,site,can_create_topic&offset=100&count=100");

            var broot = JsonConvert.DeserializeObject<RootObjectW>(wall100, new JsonSerializerSettings { Error = HandleDeserializationError });
            var resp = broot?.response;
            if (resp == null)
            {
                _log.Verbose($"Cannot parse {wall100}");
                return null;
            }
            //broot = JsonConvert.DeserializeObject<RootObjectW>(wall200, new JsonSerializerSettings { Error = HandleDeserializationError });
            //resp.count += broot?.response?.count ?? 0;
            //resp.groups.AddRange(broot?.response?.groups??new List<GroupData>());
            //resp.items.AddRange(broot?.response?.items??new List<WallPost>());
            //resp.profiles.AddRange(broot?.response?.profiles ?? new List<Profile>());
            resp.GroupId = groupId;
            return resp;
        }

        private string Request(string url, int retryCount = 5)
        {
            if (retryCount <= 0)
            {
                IsSpoiled = true;
                return string.Empty;
            }

            _holder.WaitOne(RequestsDelayMs);
            string html = string.Empty;

            var proxy = _proxy.GetProxy();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = proxy;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.85 Safari/537.36";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
                if (html.Contains("\"error_msg\":\"Too many requests per second\""))
                {
                    RequestsDelayMs = Convert.ToInt32(RequestsDelayMs * 1.2);
                    _log.Verbose($"Delay set {RequestsDelayMs}. Retry {5 - retryCount}/5.");
                    Thread.Sleep(RequestsDelayMs);
                    return Request(url, retryCount - 1);
                }
                else
                    RequestsDelayMs = Math.Max(500, Convert.ToInt32(RequestsDelayMs * 0.8));
            }
            catch (Exception ex)
            {
                _log.Error($"Error, retry {5 - retryCount}");
                Thread.Sleep(1000 * 60 * 5);
                Request(url, retryCount - 1);
            }
            finally
            {
                _proxy.PullBack(proxy, 0);
            }

            if (html.Contains("User authorization failed: invalid session."))
                IsSpoiled = true;
            return html;
        }

        class RootObjectW
        {
            public WallResponse response { get; set; }
        }

        void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
            _log.Error(currentError);
        }
    }
}