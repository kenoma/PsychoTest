using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Psycho.Gathering.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Implementations
{
    class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<ICompressor>().To<CompressorProto>();
            Bind<IUserGetRepository>().ToMethod(z => new UserGetRepository(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.Default.DbFilename),
                z.Kernel.Get<ILogger>(),
                z.Kernel.Get<ICompressor>())).InSingletonScope();
            Bind<IAntiBotRepository>().ToMethod(z => new AntiBotRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.Default.DbFilename))).InSingletonScope();
            Bind<IWallPostRepository>().ToMethod(z => new WallPostRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.Default.WallPostDatabase), z.Kernel.Get<ILogger>())).InSingletonScope();
            Bind<IReadOnlyList<UserProfileGathering>>().ToMethod(z => GetUserGetGatherings(z, config.Default.vk_keys));
            Bind<IReadOnlyList<GroupGathering>>().ToMethod(z => GetGroupGatherings(z, config.Default.vk_keys));
            Bind<Manager>().ToSelf();
            Bind<ILogger>().ToMethod(p =>
            {
                return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Seq("http://localhost:5341/", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose, compact: true)
                .WriteTo.LiterateConsole()
                .CreateLogger();
            }).InSingletonScope();
            //Bind<IProxyProvider>().ToMethod(p => new ProxyProviderBestProxy(config.Default.BestProxyURL, p.Kernel.Get<ILogger>()));//.To<DummyProxyProvider>();
            Bind<IProxyProvider>()
                //.To<DummyProxyProvider>()
                .ToMethod(p => new ProxyProvider(config.Default.FineProxyUrl, p.Kernel.Get<ILogger>()))
                .InSingletonScope();//.
            Bind<IRepoService>().To<RepoService>();
            
        }

        private static UserProfileGathering[] GetUserGetGatherings(IContext context, StringCollection configData)
        {
            var retval = new List<UserProfileGathering>();
            var proxyProvider = context.Kernel.Get<IProxyProvider>();
            var seed = 0;
            foreach (var vkKey in configData)
            {
                retval.Add(new UserProfileGathering(context.Kernel.Get<ILogger>(), context.Kernel.Get<IProxyProvider>(), vkKey) {
                    Id = seed++ });
            }
            return retval.ToArray();
        }

        private static GroupGathering[] GetGroupGatherings(IContext context, StringCollection configData)
        {
            var retval = new List<GroupGathering>();
            var proxyProvider = context.Kernel.Get<IProxyProvider>();
            var seed = 0;
            foreach (var vkKey in configData)
            {
                retval.Add(new GroupGathering(context.Kernel.Get<ILogger>(), context.Kernel.Get<IProxyProvider>(), vkKey) { Id = seed++ });
            }
            return retval.ToArray();
        }

    }
}
