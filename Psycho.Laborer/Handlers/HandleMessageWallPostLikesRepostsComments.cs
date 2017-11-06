using Psycho.Common.Service.Messages;
using Psycho.Laborer.Infrastructure;
using Psycho.Laborer.Repo;
using Psycho.Laborer.Repo.SpecialModel;
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
    class HandleMessageWallPostLikesRepostsComments : IHandleMessages<MessageWallPostLikesRepostsComments>
    {
        private readonly UnitsProvider _unitsProvider;
        private ILogger _log;
        private IBus _bus;
        private GeneralRepo _repo;

        public HandleMessageWallPostLikesRepostsComments(
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

        async public Task Handle(MessageWallPostLikesRepostsComments message)
        {
            _log.Verbose("Received message {@message}", message);
            try
            {
                var tos = new List<UserGroupActivity>();
                tos.AddRange(ExtractLikesData(message));
                tos.AddRange(ExtractRepostData(message));
                tos.AddRange(ExtractCommentsData(message));

                _repo.StoreActivity(tos);

                var cleanId = tos.Select(z => z.UserGetId).Distinct().ToArray();
                foreach (var userId in cleanId)
                {
                    await _bus.Publish(new MessageUserGet { UserGetId = userId });
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error while handling MessageWallPostLikesRepostsComments");
                throw;
            }
        }

        private List<UserGroupActivity> ExtractLikesData(MessageWallPostLikesRepostsComments message)
        {
            var tos = new List<UserGroupActivity>();
            var count = 0;
            var iter = 0;
            do
            {
                if (count < iter * 1000)
                    break;

                var requestor = _unitsProvider.GetRequestor();
                var responce = requestor.GetRequest<LikesRoot>("likes.getList",
                              new
                              {
                                  type = message.PostType,
                                  owner_id = message.OwnerId,
                                  item_id = message.WallPostId,
                                  filter = "likes",
                                  friends_only = 0,
                                  extended = 1,
                                  offset = iter++ * 1000,
                                  count = 1000,
                              });

                if (responce?.response?.items?.Any() ?? false)
                {
                    foreach (var like in responce.response.items)
                        if (like.type == "profile")
                        {
                            tos.Add(new UserGroupActivity { ActivityType = ActivityType.Like, GroupId = Math.Abs(message.OwnerId), PostType = message.PostType, WallPostId = message.WallPostId, UserGetId = like.id });
                        }
                }
                else
                    break;

                count = responce?.response?.count ?? 0;

            } while (count > 0);
            return tos;
        }

        private List<UserGroupActivity> ExtractRepostData(MessageWallPostLikesRepostsComments message)
        {
            var tos = new List<UserGroupActivity>();
            var count = 0;
            var iter = 0;
            do
            {
                if (count < iter * 1000)
                    break;

                var requestor = _unitsProvider.GetRequestor();
                var responce = requestor.GetRequest<LikesRoot>("likes.getList",
                              new
                              {
                                  type = message.PostType,
                                  owner_id = message.OwnerId,
                                  item_id = message.WallPostId,
                                  filter = "copies",
                                  friends_only = 0,
                                  extended = 1,
                                  offset = iter++ * 1000,
                                  count = 1000,
                              });

                if (responce?.response?.items?.Any() ?? false)
                {
                    foreach (var like in responce.response.items)
                        if (like.type == "profile")
                        {
                            tos.Add(new UserGroupActivity { ActivityType = ActivityType.Repost, GroupId = Math.Abs(message.OwnerId), PostType = message.PostType, WallPostId = message.WallPostId, UserGetId = like.id });
                        }
                }
                else
                    break;

                count = responce?.response?.count ?? 0;

            } while (count > 0);
            return tos;
        }

        private List<UserGroupActivity> ExtractCommentsData(MessageWallPostLikesRepostsComments message)
        {
            var tos = new List<UserGroupActivity>();

            var count = 0;
            var iter = 0;
            do
            {
                if (count < iter * 100)
                    break;

                var requestor = _unitsProvider.GetRequestor();
                var responce = requestor.GetRequest<LikesRoot>("wall.getComments",
                              new
                              {
                                  owner_id = message.OwnerId,
                                  post_id = message.WallPostId,
                                  need_likes = 0,
                                  offset = iter++ * 100,
                                  count = 100,
                                  sort = "asc",
                                  preview_length = 1,

                              });

                if (responce?.response?.items?.Any() ?? false)
                {
                    foreach (var comment in responce.response.items)
                        if (comment.from_id > 0)
                            tos.Add(new UserGroupActivity { ActivityType = ActivityType.Comment, GroupId = Math.Abs(message.OwnerId), PostType = message.PostType, WallPostId = message.WallPostId, UserGetId = comment.from_id });
                }
                else
                    break;

                count = responce?.response?.count ?? 0;

            } while (count > 0);
            return tos;
        }

        public class LikeData
        {
            public string type { get; set; }
            public int id { get; set; }
            public int from_id { get; set; }//comments
        }

        public class LikeResponse
        {
            public int count { get; set; }
            public List<LikeData> items { get; set; }
        }

        public class LikesRoot
        {
            public LikeResponse response { get; set; }
        }


    }
}
