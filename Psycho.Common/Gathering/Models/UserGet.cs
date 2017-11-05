using Newtonsoft.Json;
using ProtoBuf;
using Psycho.Common.Domain;
using Psycho.Common.Repository.Local;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Psycho.Gathering.Models
{
    [DebuggerDisplay("[{id}] {first_name} {last_name}")]
    [ProtoContract, DataContract]
    public class UserGet : ILocalAggregateRoot
    {
        [DataMember, ProtoMember(1)]
        public int id { get; set; }

        [DataMember, ProtoMember(2)]
        public string first_name { get; set; }

        [DataMember, ProtoMember(3)]
        public string last_name { get; set; }

        [DataMember, ProtoMember(4)]
        public byte sex { get; set; }

        [DataMember, ProtoMember(5)]
        public string nickname { get; set; }

        [DataMember, ProtoMember(6)]
        public string maiden_name { get; set; }

        [DataMember, ProtoMember(7)]
        public string domain { get; set; }

        [DataMember, ProtoMember(8)]
        public string screen_name { get; set; }

        [DataMember, ProtoMember(9)]
        public string photo_id { get; set; }

        [DataMember, ProtoMember(10)]
        public byte has_photo { get; set; }

        [DataMember, ProtoMember(11)]
        public byte has_mobile { get; set; }

        [DataMember, ProtoMember(12)]
        public byte friend_status { get; set; }

        [DataMember, ProtoMember(13)]
        public byte online { get; set; }

        [DataMember, ProtoMember(14)]
        public int wall_comments { get; set; }

        [DataMember, ProtoMember(15)]
        public byte can_post { get; set; }

        [DataMember, ProtoMember(16)]
        public byte can_see_all_posts { get; set; }

        [DataMember, ProtoMember(17)]
        public byte can_see_audio { get; set; }

        [DataMember, ProtoMember(18)]
        public byte can_write_private_message { get; set; }

        [DataMember, ProtoMember(19)]
        public byte can_send_friend_request { get; set; }

        [DataMember, ProtoMember(20)]
        public string site { get; set; }

        [DataMember, ProtoMember(21)]
        public string status { get; set; }

        [DataMember, ProtoMember(22)]
        public LastSeen last_seen { get; set; }

        [DataMember, ProtoMember(23)]
        public byte verified { get; set; }

        [DataMember, ProtoMember(24)]
        public int followers_count { get; set; }

        [DataMember, ProtoMember(25)]
        public int blacklisted { get; set; }

        [DataMember, ProtoMember(26)]
        public byte is_favorite { get; set; }

        [DataMember, ProtoMember(27)]
        public byte is_hidden_from_feed { get; set; }

        [DataMember, ProtoMember(28)]
        public int common_count { get; set; }

        [DataMember, ProtoMember(29)]
        public Occupation occupation { get; set; }

        [DataMember, ProtoMember(30)]
        public City city { get; set; }

        [DataMember, ProtoMember(31)]
        public Country country { get; set; }

        [DataMember, ProtoMember(32)]
        public string mobile_phone { get; set; }

        [DataMember, ProtoMember(33)]
        public List<Career> career { get; set; }

        [DataMember, ProtoMember(34)]
        public List<Military> military { get; set; }

        [DataMember, ProtoMember(35)]
        public int university { get; set; }

        [DataMember, ProtoMember(36)]
        public string university_name { get; set; }

        [DataMember, ProtoMember(37)]
        public int faculty { get; set; }

        [DataMember, ProtoMember(38)]
        public string faculty_name { get; set; }

        [DataMember, ProtoMember(39)]
        public int graduation { get; set; }

        [DataMember, ProtoMember(40)]
        public string home_town { get; set; }

        [DataMember, ProtoMember(41)]
        public int relation { get; set; }

        [DataMember, ProtoMember(42)]
        public string interests { get; set; }

        [DataMember, ProtoMember(43)]
        public string music { get; set; }

        [DataMember, ProtoMember(44)]
        public string activities { get; set; }

        [DataMember, ProtoMember(45)]
        public string movies { get; set; }

        [DataMember, ProtoMember(46)]
        public string tv { get; set; }

        [DataMember, ProtoMember(47)]
        public string books { get; set; }

        [DataMember, ProtoMember(48)]
        public string games { get; set; }

        [DataMember, ProtoMember(49)]
        public List<University> universities { get; set; }

        [DataMember, ProtoMember(50)]
        public List<School> schools { get; set; }

        [DataMember, ProtoMember(51)]
        public string about { get; set; }

        [DataMember, ProtoMember(52)]
        public List<Relative> relatives { get; set; }

        [DataMember, ProtoMember(53)]
        public string quotes { get; set; }

        [DataMember, ProtoMember(54)]
        public string home_phone { get; set; }

        [DataMember, ProtoMember(55)]
        public string instagram { get; set; }

        [DataMember, ProtoMember(56)]
        public Personal personal { get; set; }

        [DataMember, ProtoMember(57)]
        public List<GroupData> Groups { get; set; }

        [DataMember, ProtoMember(58)]
        public List<WallPost> WallPosts { get; set; }

        [DataMember, ProtoMember(59)]
        public List<Profile> profiles { get; set; }

        [DataMember, ProtoMember(60)]
        public List<GroupData> wallGroups { get; set; }

        [DataMember, ProtoMember(61)]
        public List<UserGet> Friends { get; set; }

        [DataMember, ProtoMember(62)]
        public List<UserGet> Followers { get; set; }

        [DataMember, ProtoMember(63)]
        public List<UserGet> Subscriptions { get; set; }

        [DataMember, ProtoMember(64)]
        public string bdate { get; set; }

        [DataMember, ProtoMember(65)]
        public List<int> GroupIds { get; set; }

        [DataMember, ProtoMember(66)]
        public string skype { get; set; }

        [DataMember, ProtoMember(67)]
        public string twitter { get; set; }
    }
}
