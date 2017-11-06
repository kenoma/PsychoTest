using Ninject;
using Psycho.Common.Rebus;
using Psycho.Common.Service.Messages;
using Rebus.Ninject;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Psycho.UnitTests")]
namespace Psycho.Laborer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var kernel = new StandardKernel(new Bindings()))
            {
                var log = kernel.Get<ILogger>();
                log.Information("App started");
                var _container = new NinjectContainerAdapter(kernel);
                using (var _bus = _container.DeployMessageBus($"AnyLaborer"))
                {
                    _bus.Subscribe(typeof(MessageExtractWallPostsCommand));
                    _bus.Subscribe(typeof(MessageWallPostLikesRepostsComments));
                    _bus.Subscribe(typeof(MessageUserGet));
                    _bus.Publish(new MessageExtractWallPostsCommand { OwnerId = -29534144, PostCount = 100 });
                    Console.ReadLine();
                    _bus.Unsubscribe(typeof(MessageExtractWallPostsCommand));
                    _bus.Unsubscribe(typeof(MessageWallPostLikesRepostsComments));
                    _bus.Unsubscribe(typeof(MessageUserGet));
                }
            }
        }
    }
}
