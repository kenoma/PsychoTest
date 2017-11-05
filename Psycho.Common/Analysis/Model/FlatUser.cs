using Psycho.Gathering.Models;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Psycho.Validator.models
{
    class FlatUser
    {
        static FlatUser()
        {
        }

        //public int id { get; set; }
        public byte sex { get; set; }
        public bool nickname { get; set; }
        public bool maiden_name { get; set; }
        public bool has_photo { get; set; }
        public bool has_mobile { get; set; }
        //public int CityId { get; set; }
        public bool mobile_phone { get; set; }
        public bool career { get; set; }
        public bool military { get; set; }
        //public int university { get; set; }
        //public int faculty { get; set; }
        public int age { get; set; }

        public int relation { get; set; }
        public int PersonalSmoking { get; set; }
        public int PersonalAlcohol { get; set; }
        public int PersonalPolitical { get; set; }
        public bool schools { get; set; }
        public bool universities { get; set; }
        public int relatives { get; set; }
        public bool skype { get; set; }
        public bool twitter { get; set; }
        public bool home_phone { get; set; }
        public bool instagram { get; set; }
        public int WallPostCount { get; set; }
        public int GroupsCount { get; set; }
        public int WallGroupsCount { get; set; }
        public int FriendsCount { get; set; }
        public int FollowersCount { get; set; }
        public int SubscriptionsCount { get; set; }
        public int WallComments { get; set; }
        public int LastSeenPlatform { get; set; }
        public byte can_post { get; set; }
        public byte can_see_all_posts { get; set; }
        public byte can_see_audio { get; set; }
        public byte can_write_private_message { get; set; }
        public byte can_send_friend_request { get; set; }
        public bool IsSimpleDomain { get; set; }
        public int ProfilesCount { get;set; }

        //public string interests { get; set; }
        //public string music { get; set; }
        //public string activities { get; set; }
        //public string movies { get; set; }
        //public string tv { get; set; }
        //public string books { get; set; }
        //public string games { get; set; }
        //public string about { get; set; }
        //public string quotes { get; set; }

        //public string PersonalReligion { get; set; }
        //public string status { get; set; }
        //public int graduation { get; set; }

        public FlatUser[] followers { get; set; }
        public GroupData[] Groups { get; set; }
        public FlatUser[] friends { get; set; }
    }
}
