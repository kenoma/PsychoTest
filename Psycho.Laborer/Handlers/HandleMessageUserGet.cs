using Newtonsoft.Json;
using Psycho.Common.Service.Messages;
using Psycho.Gathering.Models;
using Psycho.Laborer.Infrastructure;
using Psycho.Laborer.Repo;
using Rebus.Bus;
using Rebus.Handlers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Handlers
{
    class HandleMessageUserGet : IHandleMessages<MessageUserGet>
    {
        static private string _fields = "photo_id,verified,sex,bdate,city,country,home_town,has_photo,photo_200_origphoto_max,online,domain,has_mobile,contacts,site,education,universities,schools,status,last_seen,followers_count,common_count,occupation,nickname,relatives,relation,personal,religion,political,smoking,alcohol,life_main,people_main,inspired_by,connections,exports,wall_comments,activities,interests,music,movies,tv,books,games,about,quotes,can_post,can_see_all_posts,can_see_audio,can_write_private_message,can_send_friend_request,is_favorite,is_hidden_from_feed,timezone,screen_name,maiden_name,friend_status,career,military,blacklisted";
        private readonly UnitsProvider _unitsProvider;
        private ILogger _log;
        private IBus _bus;
        private GeneralRepo _repo;

        public HandleMessageUserGet(
            UnitsProvider unitsProvider,
            ILogger log,
            IBus bus,
            GeneralRepo repo)
        {
            _unitsProvider = unitsProvider;
            _log = log;
            _bus = bus;
            _repo = repo;
        }

        async public Task Handle(MessageUserGet message)
        {
            _log.Verbose("Received message {@message}", message);
            if (_repo.IsUserExist(message.UserGetId))
                return;

            var userGet = await RetrieveUserDataAsync(message.UserGetId);

            if (userGet == null)
                return;

            _repo.Save(userGet);
        }

        internal Task<UserGet> RetrieveUserDataAsync(int id)
        {
            return Task.Run(() =>
            {
                var requestor = _unitsProvider.GetRequestor();
                var broot = requestor.GetRequest<RootObjectUsderGet>("users.get",
                    new
                    {
                        user_id = id,
                        fields = _fields,
                    });

                if ((broot?.response?.Count ?? 0) == 0)
                    return null;

                var mother = broot.response[0];
                mother.Friends = new List<UserGet>(RetrieveUsers("friends.get", id));
                mother.Followers = new List<UserGet>(RetrieveUsers("users.getFollowers", id));

                var subsIds = requestor.GetRequest<RootObjectSubs>("users.getSubscriptions",
                    new
                    {
                        user_id = id,
                        fields = "id",
                        count = 200
                    });

                mother.Subscriptions = new List<UserGet>(RetrieveUsers(subsIds?.response?.users?.items?.Select(z => z) ?? new int[0]));
                FillGroupInfo(mother);
                FillWallInfo(mother);
                return mother;
            });
        }

        private void FillGroupInfo(UserGet user)
        {
            try
            {
                var requestor = _unitsProvider.GetRequestor();
                var broot = requestor.GetRequest<RootObjectG>("groups.get",
                    new
                    {
                        user_id = user.id,
                        extended = 1,
                        fields = _fields,
                        count = 1000
                    });

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
                    var sreqs = targs.Select(z => $"API.groups.get({{user_id:{z.id},extended:0,fields:\"id\",count:1000}})");
                    var requestor = _unitsProvider.GetRequestor();
                    var broot = requestor.GetRequest<RootObjectGG>("execute",
                        new
                        {
                            code = $"return [{string.Join(",", sreqs)}];"
                        });

                    for (int usr = 0; usr < targs.Length; usr++)
                    {
                        targs[usr].GroupIds = broot.response[usr]?.items;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
        }

        private void FillWallInfo(UserGet user)
        {
            var requestor = _unitsProvider.GetRequestor();
            var broot = requestor.GetRequest<RootObjectW>("wall.get",
                new
                {
                    owner_id = user.id,
                    extended = 1,
                    fields = _fields,
                    count = 100
                });

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
                    var requestor = _unitsProvider.GetRequestor();
                    var broot = requestor.GetRequest<RootObjectUsderGet>("users.get",
                        new
                        {
                            user_ids = strflist,
                            fields = _fields
                        });
                        
                    if ((broot?.response?.Count ?? 0) == 0)
                        continue;

                    retval.AddRange(broot.response);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }

            return retval;
        }

        private IReadOnlyList<UserGet> RetrieveUsers(string method, int userId)
        {
            var retval = new List<UserGet>();
            try
            {
                var requestor = _unitsProvider.GetRequestor();
                var idsResponce = requestor.GetRequest<RootObject>(method,
                    new
                    {
                        user_id = userId,
                        fields = "id",
                        count = 1000
                    });
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
            public List<Gathering.Models.Profile> profiles { get; set; }
            public List<GroupData> groups { get; set; }
        }

        class RootObjectW
        {
            public ResponseW response { get; set; }
        }



    }
}
