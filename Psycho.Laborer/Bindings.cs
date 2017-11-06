using Ninject.Modules;
using Psycho.Common.Repository.Local;
using Psycho.Common.Service;
using Psycho.Common.Service.Messages;
using Psycho.Laborer.Handlers;
using Psycho.Laborer.Infrastructure;
using Psycho.Laborer.Repo;
using Rebus.Handlers;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer
{
    class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().ToConstant(LogHelper.InitLog());
            Bind<IProxyProvider>().To<ProxyProvider>().InSingletonScope().WithConstructorArgument("fineproxyUrl", Config.Default.FineproxyUrl);
            Bind<UnitsProvider>().ToSelf().InSingletonScope().WithConstructorArgument("vkKeys", Config.Default.VkKeys.Cast<string>().ToArray()).WithConstructorArgument("userAgents", Config.Default.UserAgents.Cast<string>().ToArray());
            Bind<GeneralRepo>().ToMethod(z =>
            {
                var connectionString = $"Data Source={Config.Default.DBPath};Compress=True;foreign keys=false;DateTimeFormat=Ticks;UTF8Encoding=True;FailIfMissing=False;";
                if (!File.Exists(Config.Default.DBPath))
                    SqliteHelpers.EnsureCreated(connectionString);
                return new GeneralRepo(connectionString);
            });
            Bind<IHandleMessages<MessageExtractWallPostsCommand>>().To<HandleExtractWallPostsCommand>();
            Bind<IHandleMessages<MessageWallPostLikesRepostsComments>>().To<HandleMessageWallPostLikesRepostsComments>();
            Bind<IHandleMessages<MessageUserGet>>().To<HandleMessageUserGet>();
        }
    }
}
