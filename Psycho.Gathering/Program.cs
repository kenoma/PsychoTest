using Ninject;
using Psycho.Gathering.Implementations;
using Psycho.Gathering.Interfaces;
using Psycho.Gathering.Service;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Psycho.UnitTests")]
namespace Psycho.Gathering
{
    class Program
    {
        static ServiceHost serviceHost;

        static void Main(string[] args)
        {
            var kernel = new StandardKernel(new Bindings());
            var repo = kernel.Get<IUserGetRepository>();
            var arepo = kernel.Get<IAntiBotRepository>();
            var log = kernel.Get<ILogger>();
            var manager = kernel.Get<Manager>();

            //manager.StartGatheringGroupsAsync().Wait();

            //var collected = repo.GetUserVkIds();

            var files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "raw"));
            var rnd = new Random();
            var queue = new ConcurrentQueue<int>();
            var remFile = "remains.txt";
            if (File.Exists(remFile))
            {
                log.Information("Using stored remains id.");
                var listId = File.ReadAllLines(remFile).OrderBy(z => rnd.NextDouble()).Select(z => int.TryParse(z, out int a) ? a : 0).Distinct().ToArray();
                foreach (var id in listId)
                {
                    queue.Enqueue(id);
                }
            }
            else
            {
                log.Information("Getting collected id.");
                var collected = arepo.GetUserVkIds();
                log.Information("Already collected accounts: {CollectedCount}", collected.Count);
                foreach (var file in files)
                {
                    log.Information("Processing {file}", file);

                    var listId = File.ReadAllLines(file).OrderBy(z => rnd.NextDouble()).Select(z => int.TryParse(z, out int a) ? a : 0)
                    .Distinct().Except(collected).ToArray();

                    foreach (var sid in listId)
                        queue.Enqueue(sid);
                }
                File.WriteAllLines(remFile, queue.Select(z => z.ToString()));
            }


            manager.StartGathering(queue);
            File.WriteAllLines(remFile, queue.Select(z => z.ToString()));
            log.Information("Restarting...");
            // Starts a new instance of the program itself\
            var fileName = Assembly.GetExecutingAssembly().Location;
            System.Diagnostics.Process.Start(fileName);

            // Closes the current process
            Environment.Exit(0);
            //serviceHost = DeployRestfulService(kernel, config.Default.ServiceAddress);

            //Console.WriteLine("Press enter to stop app");
            //Console.ReadLine();
            //serviceHost.Close();
        }

        static private ServiceHost DeployRestfulService(IKernel kernel, string serviceAddress)
        {
            var restfulServiceAddress = new Uri($"http://{serviceAddress}/");
            Console.WriteLine(restfulServiceAddress);
            var host = new ServiceHost(typeof(RepoService), restfulServiceAddress);
            host.Description.Behaviors.Add(new NinjectBehavior(kernel, typeof(RepoService)));


            var webServiceBindings = GetBinding();

            var endpoint = host.AddServiceEndpoint(
                    typeof(IRepoService),
                    webServiceBindings,
                    restfulServiceAddress);
            endpoint.Behaviors.Add(new WebHttpBehavior
            {
                AutomaticFormatSelectionEnabled = false,
                HelpEnabled = true,
                DefaultBodyStyle = WebMessageBodyStyle.Wrapped,
                DefaultOutgoingRequestFormat = WebMessageFormat.Json,
                DefaultOutgoingResponseFormat = WebMessageFormat.Json,
                FaultExceptionEnabled = true
            });
            //endpoint.Behaviors.Add(new EnableCrossOriginResourceSharingBehavior($"http://{config.ServiceAddress}"));
            host.Open();
            return host;
        }

        class MyMapper : WebContentTypeMapper
        {
            public override WebContentFormat GetMessageFormatForContentType(string contentType)
            {
                return WebContentFormat.Json;
            }
        }

        static Binding GetBinding()
        {
            return new WebHttpBinding(WebHttpSecurityMode.None)
            {
                MaxReceivedMessageSize = int.MaxValue,
                ReaderQuotas =
                {
                    MaxArrayLength = int.MaxValue,
                    MaxStringContentLength = int.MaxValue
                },
                OpenTimeout = TimeSpan.FromDays(14),
                ReceiveTimeout = TimeSpan.FromDays(14),
                SendTimeout = TimeSpan.FromDays(14),
                CloseTimeout = TimeSpan.FromDays(14)
            };
        }

    }
}
