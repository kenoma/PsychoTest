using MongoDB.Driver;
using Newtonsoft.Json;
using Ninject;
using Serilog;
using Psycho.Common.Domain.UserData;
using Psycho.Common.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Service
{
    class Program
    {
        static Binding WebServiceBindings = new WebHttpBinding(WebHttpSecurityMode.None)
        {
            MaxReceivedMessageSize = int.MaxValue,
            ReaderQuotas =
                {
                    MaxArrayLength = int.MaxValue,
                    MaxStringContentLength = int.MaxValue
                },
            OpenTimeout = TimeSpan.FromMinutes(5),
            ReceiveTimeout = TimeSpan.FromDays(7),
            SendTimeout = TimeSpan.FromMinutes(5),
            CloseTimeout = TimeSpan.FromDays(7)
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Starting app...");
            var kernel = new StandardKernel(new Bindings());

            var restfulServiceAddress = new Uri(config.Default.ServiceEndpoint);

            //var serviceHost = new ServiceHost(kernel.Get<IQuestionnaireService>(), restfulServiceAddress);
            var serviceHost  = new ServiceHost(typeof(QuestionnaireService), restfulServiceAddress);
            serviceHost.Description.Behaviors.Add(new NinjectBehavior(kernel, typeof(QuestionnaireService)));

            var endpoint = serviceHost.AddServiceEndpoint(
                typeof(IQuestionnaireService),
                WebServiceBindings,
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

            //ExportDataBase(kernel);

            serviceHost.Open();
            Console.WriteLine($"Service deployed at {restfulServiceAddress}");
            Console.WriteLine("Press any key to close app.");
            Console.ReadLine();
        }

        private static void ExportDataBase(StandardKernel kernel)
        {
            var db = kernel.Get<IMongoDatabase>();
            var rcollection = db.GetCollection<RespondentUser>(nameof(RespondentUser));

            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "export")))
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "export"));

            foreach (var user in rcollection.Find(FilterDefinition<RespondentUser>.Empty).ToList())
                if (user?.DataVkontakte != null)
                {
                    var oldid = user.VkId;

                    user.DataVkontakte.UserId = int.Parse(Reverse((user.DataVkontakte.UserId * 2).ToString() + "1"));
                    user.VkId = user.DataVkontakte.UserId;
                    var resp = JsonConvert.DeserializeObject<RootObject>(user.DataVkontakte.UsersGet);

                    resp.response.First().id = user.DataVkontakte.UserId;
                    resp.response.First().first_name = "";
                    resp.response.First().last_name = "";
                    user.DataVkontakte.UsersGet = JsonConvert.SerializeObject(resp);
                    user.DataVkontakte.UsersGet = user.DataVkontakte.UsersGet.Replace(oldid.ToString(), "");

                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "export", $"{user.DataVkontakte.UserId}.json"), JsonConvert.SerializeObject(user));
                    Console.WriteLine($"Id {user.DataVkontakte.UserId}");
                }
        }

        static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }


        public class LastSeen
        {
            public int time { get; set; }
            public int platform { get; set; }
        }

        public class Response
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public int sex { get; set; }
            public string nickname { get; set; }
            public string domain { get; set; }
            public string screen_name { get; set; }
            public string bdate { get; set; }
            public int timezone { get; set; }
            public string photo_50 { get; set; }
            public string photo_100 { get; set; }
            public string photo_200 { get; set; }
            public string photo_max { get; set; }
            public string photo_200_orig { get; set; }
            public string photo_400_orig { get; set; }
            public string photo_max_orig { get; set; }
            public int has_photo { get; set; }
            public int has_mobile { get; set; }
            public int is_friend { get; set; }
            public int friend_status { get; set; }
            public int online { get; set; }
            public int wall_comments { get; set; }
            public int can_post { get; set; }
            public int can_see_all_posts { get; set; }
            public int can_see_audio { get; set; }
            public int can_write_private_message { get; set; }
            public int can_send_friend_request { get; set; }
            public string home_phone { get; set; }
            public string site { get; set; }
            public string status { get; set; }
            public LastSeen last_seen { get; set; }
            public int verified { get; set; }
            public int followers_count { get; set; }
            public int blacklisted { get; set; }
            public int blacklisted_by_me { get; set; }
            public int is_favorite { get; set; }
            public int is_hidden_from_feed { get; set; }
            public int common_count { get; set; }
            public List<object> career { get; set; }
            public List<object> military { get; set; }
            public int university { get; set; }
            public string university_name { get; set; }
            public int faculty { get; set; }
            public string faculty_name { get; set; }
            public int graduation { get; set; }
            public string home_town { get; set; }
            public int relation { get; set; }
            public string interests { get; set; }
            public string music { get; set; }
            public string activities { get; set; }
            public string movies { get; set; }
            public string tv { get; set; }
            public string books { get; set; }
            public string games { get; set; }
            public List<object> universities { get; set; }
            public List<object> schools { get; set; }
            public string about { get; set; }
            public List<object> relatives { get; set; }
            public string quotes { get; set; }
        }

        public class RootObject
        {
            public List<Response> response { get; set; }
        }
    }
}
