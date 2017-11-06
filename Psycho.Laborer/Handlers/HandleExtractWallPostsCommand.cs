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
    class HandleExtractWallPostsCommand : IHandleMessages<MessageExtractWallPostsCommand>
    {
        private readonly UnitsProvider _unitsProvider;
        private ILogger _log;
        private IBus _bus;

        public HandleExtractWallPostsCommand(
            UnitsProvider unitsProvider, 
            ILogger log,
            IBus bus)
        {
            _unitsProvider = unitsProvider;
            _log = log;
            _bus = bus;
        }

        async public Task Handle(MessageExtractWallPostsCommand message)
        {
            _log.Verbose("Received message {@message}", message);
            var requestor = _unitsProvider.GetRequestor();

            var wallpostRequests = Enumerable.Range(0, message.PostCount / 100).Select(z => $"API.wall.get({{owner_id:{message.OwnerId},extended:0,offset:{z * 100},count:100}})");
            var responce = requestor.GetRequest<PWallResponces>("execute",
                          new
                          {
                              code = $"return [{string.Join(",", wallpostRequests)}];"
                          });

            if (responce?.response?.Any() ?? false)
                foreach (var resp in responce.response)
                {
                    if (resp?.items?.Any() ?? false)
                        foreach (var wallpost in resp.items)
                        {
                            await _bus.Publish(new MessageWallPostLikesRepostsComments
                            {
                                OwnerId = message.OwnerId,
                                WallPostId = wallpost.id,
                                PostType = wallpost.post_type
                            });
                            return;
                        }
                }
        }
        
        public class PWallResponces
        {
            public List<WallResponse> response { get; set; }
        }
    }
}
