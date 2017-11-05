using Newtonsoft.Json;
using Serilog;
using Psycho.Common.Domain.UserData;
using Psycho.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Psycho.Service.Implementations
{
    class GrabberVK : ISocialNetworkGrabber<SocialNetworkDataVkontakte>
    {
        private readonly ILogger _log;
        private static int RequestsDelayMs = 400;
        private DateTime _lastCall = DateTime.Now;
        private static object _locker = new object();

        public GrabberVK(ILogger log)
        {
            _log = log;
        }

        public SocialNetworkDataVkontakte GetUserData(string vkAccessToken)
        {
            _log.Information($"Retrieving vk data for {vkAccessToken}...");
            SocialNetworkDataVkontakte vkData = new SocialNetworkDataVkontakte();

            vkData.UsersGet = Request($"https://api.vk.com/method/users.get?v=5.52&access_token={vkAccessToken}&fields=photo_id,verified,sex,bdate,city,country,home_town,has_photo,photo_50,photo_100,photo_200_orig,photo_200,photo_400_orig,photo_max,photo_max_orig,online,domain,has_mobile,contacts,site,education,universities,schools,status,last_seen,followers_count,common_count,occupation,nickname,relatives,relation,personal,connections,exports,wall_comments,activities,interests,music,movies,tv,books,games,about,quotes,can_post,can_see_all_posts,can_see_audio,can_write_private_message,can_send_friend_request,is_favorite,is_hidden_from_feed,timezone,screen_name,maiden_name,crop_photo,is_friend,friend_status,career,military,blacklisted,blacklisted_by_me,political");

            if (string.IsNullOrEmpty(vkData.UsersGet))
                throw new Exception($"Failed to get data for {vkAccessToken}");
            var root = JsonConvert.DeserializeObject<UserRoot>(vkData.UsersGet);
            if (root?.response?.FirstOrDefault() != null)
                vkData.UserId = root.response.FirstOrDefault().id;

            var requests = new string[]
                {
                    "API.users.getFollowers({count:1000, fields:\"photo_id,verified,sex,bdate,city,country,home_town,has_photo,photo_50,photo_100,photo_200_orig,photo_200,photo_400_orig,photo_max,photo_max_orig,online,lists,domain,has_mobile,contacts,site,education,universities,schools,status,last_seen,followers_count,common_count,occupation,nickname,relatives,relation,personal,connections,exports,wall_comments,activities,interests,music,movies,tv,books,games,about,quotes,can_post,can_see_all_posts,can_see_audio,can_write_private_message,can_send_friend_request,is_favorite,is_hidden_from_feed,timezone,screen_name,maiden_name,crop_photo,is_friend,friend_status,career,military,blacklisted,blacklisted_by_me\"})",
                    "API.users.getSubscriptions({count:200, extended:1})",
                    "API.docs.get({count:2000, type:0})",
                    //"API.fave.getLinks({count:50})",
                    //"API.fave.getMarketItems({count:50, extended:1})",
                    //"API.fave.getPhotos({count:50})",
                    //"API.fave.getPosts({extended:1, count:50})",
                    //"API.fave.getUsers({count:50})",
                    //"API.fave.getVideos({extended:1, count:50})",
                    "API.friends.get({order:\"name\", fields:\"nickname,domain,sex,bdate,city,country,timezone,photo_50,photo_100,photo_200_orig,has_mobile,contacts,education,online,relation,last_seen,status,can_write_private_message,can_see_all_posts,can_post,universities\"})",
                    "API.friends.getRecent({count:1000})",
                    //"API.friends.getRequests({count:1000, extended:1, need_viewed:1})",
                    //"API.gifts.get({count:1000})",
                    "API.groups.get({extended:1, fields:\"city,country,place,description,wiki_page,members_count,counters,start_date,finish_date,can_post,can_see_all_posts,activity,status,contacts,links,fixed_post,verified,site,can_create_topic\"})",
                    //"API.newsfeed.getLists({extended:1})",
                    "API.notes.get({count:100})",
                    "API.photos.getAlbums({need_system:1})",
                    "API.photos.get({extended:1, album_id:\"wall\"})",
                    "API.photos.get({extended:1, album_id:\"profile\"})",
                    "API.photos.get({extended:1, album_id:\"saved\"})",
                    "API.photos.getAlbumsCount()",
                    "API.photos.getAll({extended:1, count:200, no_service_albums:0, need_hidden:1})",
                    "API.photos.getAllComments({need_likes:1, count:100})",
                    "API.photos.getUserPhotos({extended:1, count:1000})",
                    "API.status.get()",
                    "API.video.get({count:100, extended:1})",
                    "API.video.getAlbums({count:100, extended:1})",
                    "API.wall.get({count:100, extended:1, fields:\"nickname,domain,sex,bdate,city,country,timezone,photo_50,photo_100,photo_200_orig,has_mobile,contacts,education,online,relation,last_seen,status,can_write_private_message,can_see_all_posts,can_post,universities,city,country,place,description,wiki_page,members_count,counters,start_date,finish_date,can_post,can_see_all_posts,activity,status,contacts,links,fixed_post,verified,site,can_create_topic\"})",
                    //"API.newsfeed.get({filters:\"post,photo,photo_tag,wall_photo,friend,note,audio,video\", count:100})",
                    //"API.newsfeed.getMentions({count:50})",
                    //"API.newsfeed.getRecommended({count:50, fields:\"sex,bdate,city,country,photo_50,photo_100,photo_200_orig,photo_200,photo_400_orig,photo_max,photo_max_orig,online,online_mobile,domain,has_mobile,contacts,connections,site,education,universities,schools,can_post,can_see_all_posts,can_see_audio,can_write_private_message,status,last_seen,common_count,relation,relatives,counters,screen_name,maiden_name,timezone,occupation,activities,interests,music,movies,tv,books,games,about,quotes\"})",
                    "API.newsfeed.getSuggestedSources({count:50, fields:\"sex,bdate,city,country,photo_50,photo_100,photo_200_orig,photo_200,photo_400_orig,photo_max,photo_max_orig,online,online_mobile,domain,has_mobile,contacts,connections,site,education,universities,schools,can_post,can_see_all_posts,can_see_audio,can_write_private_message,status,last_seen,common_count,relation,relatives,counters,screen_name,maiden_name,timezone,occupation,activities,interests,music,movies,tv,books,games,about,quotes\"})",
                    //"API.notifications.get({count:100, filters:\"wall,mentions,comments,likes,reposts,followers,friends\"})",
                    "API.docs.getTypes()",
                    "API.friends.getOnline({online_mobile:1})"
                };

            vkData.RequestOne = Request($"https://api.vk.com/method/execute?v=5.52&access_token={vkAccessToken}&code=return {{{string.Join(",\r\n", requests.Take(10).Select((x, i) => $"req_{i}:{x}"))}}};");
            vkData.RequestTwo = Request($"https://api.vk.com/method/execute?v=5.52&access_token={vkAccessToken}&code=return {{{string.Join(",\r\n", requests.Skip(10).Take(25).Select((x, i) => $"req_{i}:{x}"))}}};");

            vkData.WallComments = GetWallComments(vkAccessToken);
            vkData.PhotoWall = GetPhotoData($"https://api.vk.com/method/photos.get?v=5.52&access_token={vkAccessToken}&album_id=wall", vkAccessToken, vkData.UserId);
            vkData.PhotoProfile = GetPhotoData($"https://api.vk.com/method/photos.get?v=5.52&access_token={vkAccessToken}&album_id=profile", vkAccessToken, vkData.UserId);
            vkData.PhotoSaved = GetPhotoData($"https://api.vk.com/method/photos.get?v=5.52&access_token={vkAccessToken}&album_id=saved", vkAccessToken, vkData.UserId);

            vkData.NotesGetComments = GetNotesComments(vkAccessToken);

            _log.Information("Data vk {vkData?.UserId} ready.");
            return vkData;
        }

        private string Request(string url, int retryCount = 5)
        {
            lock (_locker)
            {
                if (retryCount <= 0)
                {
                    _log.Warning("Retry counter excided!");
                    return string.Empty;
                }
                var elapsed = (DateTime.Now - _lastCall).TotalMilliseconds;
                if (elapsed < RequestsDelayMs)
                    Thread.Sleep(Convert.ToInt32(RequestsDelayMs - elapsed));

                string html = string.Empty;
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.85 Safari/537.36";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        _lastCall = DateTime.Now;
                        html = reader.ReadToEnd();
                    }

                    if (html.Contains("\"error_msg\":\"Too many requests per second\""))
                    {
                        _log.Warning($"Delay set {RequestsDelayMs}. Retry {5 - retryCount}/5.");
                        Thread.Sleep(RequestsDelayMs);
                        return Request(url, retryCount - 1);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"Error, retry {5 - retryCount}: {url} : {ex.Message}");
                    Thread.Sleep(2 * RequestsDelayMs);
                    Request(url, retryCount - 1);
                }

                if (html.Contains("User authorization failed: invalid session."))
                    _log.Warning("User authorization failed: invalid session.");

                return html;
            }
        }

        private string GetPhotoData(string request, string vkAccessToken, int userId)
        {
            string results = "";
            try
            {
                var photos = Request(request);
                dynamic tempora = JsonConvert.DeserializeObject<dynamic>(photos);
                var listId = new List<dynamic>();
                var listTags = new List<dynamic>();
                var root = JsonConvert.DeserializeObject<RootObject>(photos);
                if (root?.response?.items?.Any() == true)
                {
                    for (int req = 0; req < root.response.items.Count; req += 10)
                    {
                        var photosgetByIds = root.response.items.Skip(req).Take(10).Select(z => $"API.photos.getById({{extended:1, photos:\"{userId}_{z.id}\"}})");
                        var photosgetTagss = root.response.items.Skip(req).Take(10).Select(z => $"API.photos.getTags({{photo_id:{z.id}}})");


                        var photosgetById = Request($"https://api.vk.com/method/execute?v=5.52&access_token={vkAccessToken}&code=return {{{string.Join(",\r\n", photosgetByIds.Select((x, i) => $"photoreq_{i}:{x}"))}}};");
                        var photosgetTags = Request($"https://api.vk.com/method/execute?v=5.52&access_token={vkAccessToken}&code=return {{{string.Join(",\r\n", photosgetTagss.Select((x, i) => $"tagreq_{i}:{x}"))}}};");

                        listId.Add(JsonConvert.DeserializeObject<dynamic>(photosgetById));
                        listTags.Add(JsonConvert.DeserializeObject<dynamic>(photosgetTags));
                    }
                }
                dynamic aggregate = new ExpandoObject();
                aggregate.Main = tempora;
                aggregate.ListId = listId;
                aggregate.listTags = listTags;

                results = JsonConvert.SerializeObject(aggregate);
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
            return results;
        }

        private string[] GetWallComments(string vkAccessToken)
        {
            var jsonWallGet = Request($"https://api.vk.com/method/wall.get?v=5.52&access_token={vkAccessToken}&count=100&extended=1&fields=nickname,domain,sex,bdate,city,country,timezone,photo_50,photo_100,photo_200_orig,has_mobile,contacts,education,online,relation,last_seen,status,can_write_private_message,can_see_all_posts,can_post,universities,city,country,place,description,wiki_page,members_count,counters,start_date,finish_date,can_post,can_see_all_posts,activity,status,contacts,links,fixed_post,verified,site,can_create_topic");
            List<string> results = new List<string> { jsonWallGet };
            try
            {
                var root = JsonConvert.DeserializeObject<RootObject>(jsonWallGet);
                var posts = new List<int>();

                if (root?.response?.items?.Any() == true)
                    posts.AddRange(root.response.items.Select(z => z.id));
                if (posts.Any())
                {
                    for (int req = 0; req < posts.Count; req += 10)
                    {
                        var sreqs = posts.Skip(req).Take(10).Select(z => $"API.wall.getComments({{need_likes:1, count:100, extended:1, post_id:{z}}})");
                        var responce = Request($"https://api.vk.com/method/execute?v=5.52&access_token={vkAccessToken}&code=return {{{string.Join(",\r\n", sreqs.Select((x, i) => $"wallreq_{i}:{x}"))}}};");
                        results.Add(responce);
                    }
                }
                return results.ToArray();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
            return null;
        }

        private string GetNotesComments(string vkAccessToken)
        {
            List<dynamic> results = new List<dynamic>();
            try
            {
                var notes = Request($"https://api.vk.com/method/wall.get?v=5.52&access_token={vkAccessToken}&count=100");
                var root = JsonConvert.DeserializeObject<RootObject>(notes);
                var comments = new List<int>();

                if (root?.response?.items?.Any() == true)
                    comments.AddRange(root.response.items.Select(z => z.id));

                if (comments.Any())
                {
                    for (int req = 0; req < comments.Count; req += 10)
                    {
                        var sreqs = comments.Skip(req).Take(10).Select(z => $"API.notes.getComments({{note_id:{z}, count:100}})");
                        var responce = Request($"https://api.vk.com/method/execute?v=5.52&access_token={vkAccessToken}&code=return {{{string.Join(",\r\n", sreqs.Select((x, i) => $"notesreq_{i}:{x}"))}}};");
                        results.Add(JsonConvert.DeserializeObject<dynamic>(responce));
                    }
                }
                return JsonConvert.SerializeObject(results);
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
            return "";
        }

        class Item
        {
            public int id { get; set; }
        }

        class Response
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public int count { get; set; }
            public List<Item> items { get; set; }
        }

        class RootObject
        {
            public Response response { get; set; }
        }

        class UserRoot
        {
            public List<Response> response { get; set; }
        }

    }
}
