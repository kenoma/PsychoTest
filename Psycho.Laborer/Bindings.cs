using Ninject.Modules;
using Psycho.Common.Service;
using Psycho.Common.Service.Messages;
using Psycho.Laborer.Handlers;
using Psycho.Laborer.Infrastructure;
using Rebus.Handlers;
using Serilog;
using System;
using System.Collections.Generic;
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
            Bind<UnitsProvider>().ToSelf().InSingletonScope().WithConstructorArgument("vkKeys", Config.Default.VkKeys).WithConstructorArgument("userAgents", Config.Default.UserAgents);
            Bind<IHandleMessages<MessageGetGroupLikesRepostsComments>>().To<HandleGetGroupLikesRepostsComments>();
        }
    }
}
