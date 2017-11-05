using Facebook;
using Ninject;
using Ninject.Modules;
using Psycho.Common.Repository;
using Psycho.Common.Service;
using Psycho.Service.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Psycho.Common.Domain.UserData;
using Psycho.Service.Interfaces;
using Serilog;
using Destructurama;

namespace Psycho.Service
{
    class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<MongoClient>().ToSelf().WithConstructorArgument("connectionString", config.Default.mongodb);
            Bind<IMongoDatabase>().ToMethod(ctx => ctx.Kernel.Get<MongoClient>().GetDatabase("psychodb"));
            Bind<IServiceUserRepository>().To<DummyServiceUserRepository>();
            Bind<IUserValidation>().To<UserValidation>();
            Bind<IRespondentsRepository>().To<RespondentsRepository>();
            Bind<IQuestionnaireRepository>().To<QuestionnaireRepository>();
            Bind<ILogger>().ToMethod(p =>
            {
                return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Destructure.JsonNetTypes()
                .WriteTo.Seq("http://localhost:5341/", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose, compact: true)
                .WriteTo.LiterateConsole()
                .CreateLogger();
            }).InSingletonScope();

            Bind<ISocialNetworkGrabber<SocialNetworkDataVkontakte>>().To<GrabberVK>();
            Bind<ISocialNetworkGrabber<SocialNetworkDataFacebook>>().To<GrabberFB>();
            Bind<GatheringManager<SocialNetworkDataFacebook>>().ToSelf().InSingletonScope();
            Bind<GatheringManager<SocialNetworkDataVkontakte>>().ToSelf().InSingletonScope();
            Bind<IQuestionnaireService>().To<QuestionnaireService>();
            Bind<CongruenceOutcomeComputer>().ToSelf();
            Bind<IOutcomeComputer>().To<OutcomeComputer>();
        }
    }
}
