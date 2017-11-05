using Newtonsoft.Json;
using Psycho.Gathering.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Psycho.Gathering.Implementations
{
    class UserProfileGathering
    {
        private string _fields = "photo_id,verified,sex,bdate,city,country,home_town,has_photo,photo_200_origphoto_max,online,domain,has_mobile,contacts,site,education,universities,schools,status,last_seen,followers_count,common_count,occupation,nickname,relatives,relation,personal,religion,political,smoking,alcohol,life_main,people_main,inspired_by,connections,exports,wall_comments,activities,interests,music,movies,tv,books,games,about,quotes,can_post,can_see_all_posts,can_see_audio,can_write_private_message,can_send_friend_request,is_favorite,is_hidden_from_feed,timezone,screen_name,maiden_name,friend_status,career,military,blacklisted";
        private readonly ILogger _log;
        private int RequestsDelayMs = 500;
        private readonly string _vkAccessToken = "";
        private readonly IProxyProvider _proxyProvider;
        static private Random _rnd = new Random(Environment.TickCount);
        public int Processed { get; set; }

        public bool IsSpoiled { get; private set; } = false;
        public int Id { get; set; }

        public UserProfileGathering(ILogger log, IProxyProvider proxyProvider, string vkAccessToken)
        {
            _vkAccessToken = vkAccessToken ?? throw new ArgumentException(nameof(vkAccessToken));
            _log = log ?? throw new ArgumentException(nameof(log));
            _proxyProvider = proxyProvider;
        }

        internal UserGet RetrieveUserData(int id)
        {
            //var sw = Stopwatch.StartNew();
            var request = Request($"https://api.vk.com/method/users.get?v=5.52&access_token={_vkAccessToken}&user_id={id}&fields={_fields}");
            var broot = JsonConvert.DeserializeObject<RootObjectUsderGet>(request, new JsonSerializerSettings { Error = HandleDeserializationError });
            //_log.Information("Request 1 {StageOneTimeMs}", sw.ElapsedMilliseconds);
            //sw = Stopwatch.StartNew();
            if ((broot?.response?.Count ?? 0) == 0)
            {
                //_log.Warn($"Failed to get data for {id}|{_vkAccessToken}");
                //IsSpoiled = true;
                //throw new Exception($"No mothership for {id}");
                return null;
            }
            var mother = broot.response[0];
            mother.Friends = new List<UserGet>(RetrieveUsers($"https://api.vk.com/method/friends.get?v=5.52&access_token={_vkAccessToken}&user_id={id}&fields=sex"));
            //_log.Information("Request 2 {StageTwoTimeMs}", sw.ElapsedMilliseconds);
            //sw = Stopwatch.StartNew();
            mother.Followers = new List<UserGet>(RetrieveUsers($"https://api.vk.com/method/users.getFollowers?v=5.52&access_token={_vkAccessToken}&user_id={id}&fields=sex"));
            //_log.Information("Request 3 {StageThreeTimeMs}", sw.ElapsedMilliseconds);
            //sw = Stopwatch.StartNew();
            var subsResponce = Request($"https://api.vk.com/method/users.getSubscriptions?v=5.52&access_token={_vkAccessToken}&user_id={id}&fields=sex&count=200");
            var subsIds = JsonConvert.DeserializeObject<RootObjectSubs>(subsResponce, new JsonSerializerSettings { Error = HandleDeserializationError });
            mother.Subscriptions = new List<UserGet>(RetrieveUsers(subsIds?.response?.users?.items?.Select(z => z) ?? new int[0]));
            //_log.Information("Request 4 {StageFourTimeMs}", sw.ElapsedMilliseconds);
            //sw = Stopwatch.StartNew();
            FillGroupInfo(mother);
            //_log.Information("Request 5 {StageFiveTimeMs}", sw.ElapsedMilliseconds);
            //sw = Stopwatch.StartNew();
            ///отключил для краткого поиска
            //FillConcieseGroupInfo(mother.Friends);
            //_log.Information("Request 6 {StageSixTimeMs}", sw.ElapsedMilliseconds);
            //sw = Stopwatch.StartNew();
            //FillConcieseGroupInfo(mother.Followers);
            //_log.Information("Request 7 {StageSevenTimeMs}", sw.ElapsedMilliseconds);
            //sw = Stopwatch.StartNew();
            //FillConcieseGroupInfo(mother.Subscriptions);
            //_log.Information("Request 8 {StageEightTimeMs}", sw.ElapsedMilliseconds);
            //sw = Stopwatch.StartNew();
            FillWallInfo(mother);
            //_log.Information("Request 9 {StageNineTimeMs}", sw.ElapsedMilliseconds);
            return mother;

        }

        private void FillGroupInfo(UserGet user)
        {
            try
            {
                var groupget = Request($"https://api.vk.com/method/groups.get?v=5.52&access_token={_vkAccessToken}&user_id={user.id}&extended=1&fields=city,country,place,description,wiki_page,members_count,counters,start_date,finish_date,can_post,can_see_all_posts,activity,status,contacts,links,fixed_post,verified,site,can_create_topic&count=1000");
                var broot = JsonConvert.DeserializeObject<RootObjectG>(groupget, new JsonSerializerSettings { Error = HandleDeserializationError });

                user.Groups = broot?.response?.items;
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
        }

        private void FillConcieseGroupInfo(List<UserGet> users)
        {
            try
            {
                for (int req = 0; req < users.Count; req += 25)
                {
                    var targs = users.Skip(req).Take(25).ToArray();
                    var sreqs = targs.Select(z => $"API.groups.get({{user_id:{z.id}, extended:0, fields:\"id\", count:1000}})");
                    var responce = Request($"https://api.vk.com/method/execute?v=5.52&access_token={_vkAccessToken}&code=return [{string.Join(",", sreqs)}];");
                    responce = responce.Replace("false", "null");
                    var broot = JsonConvert.DeserializeObject<RootObjectGG>(responce, new JsonSerializerSettings { Error = HandleDeserializationError });
                    for (int usr = 0; usr < targs.Length; usr++)
                    {
                        targs[usr].GroupIds = broot.response[usr]?.items;
                    }
                }
                //var groupget = Request($"https://api.vk.com/method/groups.get?v=5.52&access_token={_vkAccessToken}&user_id={user.id}&fields=id&count=1000");

            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
        }

        private void FillWallInfo(UserGet user)
        {
            var wall = Request($"https://api.vk.com/method/wall.get?v=5.52&access_token={_vkAccessToken}&owner_id={user.id}&extended=1&fields=city,country,place,description,wiki_page,members_count,counters,start_date,finish_date,can_post,can_see_all_posts,activity,status,contacts,links,fixed_post,verified,site,can_create_topic&count=100");

            var broot = JsonConvert.DeserializeObject<RootObjectW>(wall, new JsonSerializerSettings { Error = HandleDeserializationError });

            user.WallPosts = broot?.response?.items;
            user.profiles = broot?.response?.profiles;
            user.wallGroups = broot?.response?.groups;
        }

        static int usersPerReq = 330;
        private IReadOnlyList<UserGet> RetrieveUsers(IEnumerable<int> flist)
        {
            var retval = new List<UserGet>();
            if ((flist?.Count() ?? 0) == 0)
                return retval;
            try
            {
                for (int i = 0; i < flist.Count(); i += usersPerReq)
                {
                    var strflist = string.Join(",", flist.Skip(i).Take(usersPerReq));
                    var request = Request($"https://api.vk.com/method/users.get?v=5.52&access_token={_vkAccessToken}&user_ids={strflist}&fields=={_fields}");
                    var broot = JsonConvert.DeserializeObject<RootObjectUsderGet>(request, new JsonSerializerSettings { Error = HandleDeserializationError });
                    if ((broot?.response?.Count ?? 0) == 0)
                    {
                        //_log.Warning($"Failed to get data for {strflist}");
                        continue;
                    }
                    retval.AddRange(broot.response);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }

            return retval;
        }

        private IReadOnlyList<UserGet> RetrieveUsers(string idsRequest)
        {
            var retval = new List<UserGet>();
            var request = Request(idsRequest);
            try
            {
                var idsResponce = JsonConvert.DeserializeObject<RootObject>(request, new JsonSerializerSettings { Error = HandleDeserializationError });
                var flist = idsResponce?.response?.items?.Select(z => z.id).ToArray();
                if (flist != null)
                    retval.AddRange(RetrieveUsers(flist));
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }

            return retval;
        }

        private DateTime _lastCall = DateTime.Now;
        private string Request(string url, int retryCount = 3)
        {
            if (retryCount <= 0)
            {
                IsSpoiled = true;
                return string.Empty;
            }
            var elapsed = (DateTime.Now - _lastCall).TotalMilliseconds;
            if (elapsed < RequestsDelayMs)
                Thread.Sleep(Math.Min(RequestsDelayMs, Convert.ToInt32(RequestsDelayMs - elapsed)));

            var html = string.Empty;
            var proxy = _proxyProvider.GetProxy();
            var sw = Stopwatch.StartNew();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ServicePoint.Expect100Continue = false;
                request.Proxy = proxy;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.UserAgent = config.Default.UserAgents[_rnd.Next(config.Default.UserAgents.Count)];

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    _lastCall = DateTime.Now;
                    html = reader.ReadToEnd();
                }

                if (html.Contains("\"error_msg\":\"Too many requests per second\""))
                {
                    _log.Warning($"[{Id}] Delay set {RequestsDelayMs}. Retry {retryCount}.");
                    Thread.Sleep(10000);
                    return Request(url, retryCount - 1);
                }
            }
            catch (WebException wex)
            {
                Thread.Sleep((4 - retryCount) * 60 * 1000);
                return Request(url, retryCount - 1);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed Vkkey: {FailedVkKey} Proxy {FailedProxy}", _vkAccessToken, (proxy as WebProxy).Address);
                //Thread.Sleep((15 - retryCount) * 200 * RequestsDelayMs);
                Thread.Sleep((4 - retryCount) * 60 * 1000);
                return Request(url, retryCount - 1);
            }
            finally
            {
                _proxyProvider.PullBack(proxy, sw.ElapsedMilliseconds);
            }


            if (html.Contains("User authorization failed: invalid session."))
            {
                IsSpoiled = true;
            }
            //_log.Information("Request {RequestURL} takes {RequestTime}", url, sw.ElapsedMilliseconds);
            return html;
        }

        private class RootObjectUsderGet
        {
            public List<UserGet> response { get; set; }
        }

        class Item
        {
            public int id { get; set; }
        }

        class Response
        {
            public int count { get; set; }
            public List<Item> items { get; set; }
        }

        class RootObject
        {
            public Response response { get; set; }
        }


        class Users
        {
            public int count { get; set; }
            public List<int> items { get; set; }
        }

        class ResponseSubs
        {
            public Users users { get; set; }
        }

        class RootObjectSubs
        {
            public ResponseSubs response { get; set; }
        }


        class ResponseGG
        {
            public int count { get; set; }
            public List<int> items { get; set; }
        }

        class ResponseG
        {
            public int count { get; set; }
            public List<GroupData> items { get; set; }
        }

        class RootObjectGG
        {
            public ResponseGG[] response { get; set; }
        }

        class RootObjectG
        {
            public ResponseG response { get; set; }
        }

        class ResponseW
        {
            public int count { get; set; }
            public List<WallPost> items { get; set; }
            public List<Models.Profile> profiles { get; set; }
            public List<GroupData> groups { get; set; }
        }

        class RootObjectW
        {
            public ResponseW response { get; set; }
        }

        void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
            _log.Error(currentError);
        }
    }
}

