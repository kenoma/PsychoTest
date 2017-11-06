using Psycho.Common.Service;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Compression;
using Rebus.Config;
using Rebus.DataBus;
using Rebus.DataBus.FileSystem;
using Rebus.Encryption;
using Rebus.Retry.Simple;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Rebus
{
    public static class ServiceBusHelper
    {
        /// <summary>
        /// Deploys the one way message bus.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="serviceBusConfig">The service bus configuration.</param>
        /// <returns></returns>
        public static IBus DeployOneWayMessageBus(this IContainerAdapter container)
        {
            return Configure.With(container)
                 .Logging(l => l.Use(new RebusLoggerFactory()))
                 .Serialization(s =>
                 s.UseProtobuf()
                 )
                 .Transport(t => t.UseRabbitMqAsOneWayClient(config.Default.RabbitMqConnectionString))
                 .Options(o =>
                 {
                     o.EnableEncryption(config.Default.SecretKey);
                     o.EnableCompression(2048);
                     o.SimpleRetryStrategy(maxDeliveryAttempts: 3);
                     o.SetMaxParallelism(config.Default.ServiceBusMaxParallelism);
                     o.SetNumberOfWorkers(config.Default.ServiceBusNumberOfWorkers);
                 })
                 .Start();
        }

        private static void ErrorHandling(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            Log.Error("JSON error {@ErrorObject}", CallerInfo.Create(), e);
        }

        /// <summary>
        /// Deploys the message bus.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="serviceBusConfig">The service bus configuration.</param>
        /// <param name="consumer">The consumer.</param>
        /// <param name="purgeQueue">todo: describe purgeQueue parameter on DeployMessageBus</param>
        /// <returns></returns>
        public static IBus DeployMessageBus(this IContainerAdapter container,
                                            string consumer,
                                            bool purgeQueue = false)
        {
            Log.Verbose("Message bus for {consumer} deploying", CallerInfo.Create(), consumer);
            return Configure.With(container)
                 .Logging(l => l.Use(new RebusLoggerFactory()))
                 .Serialization(s =>s.UseProtobuf())
                 .Transport(t => t.UseRabbitMq(config.Default.RabbitMqConnectionString, consumer))
                 .Options(o =>
                 {
                     o.EnableEncryption(config.Default.SecretKey);
                     o.EnableCompression(2048);
                     o.SimpleRetryStrategy(maxDeliveryAttempts: 3);
                     o.SetMaxParallelism(config.Default.ServiceBusMaxParallelism);
                     o.SetNumberOfWorkers(config.Default.ServiceBusNumberOfWorkers);
                 })
                 .Start();
        }
    }
}
