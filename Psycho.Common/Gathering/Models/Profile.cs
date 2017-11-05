using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Models
{
    [ProtoContract, DataContract]
    public class Profile
    {
        [DataMember, ProtoMember(1)]
        public int id { get; set; }
        [DataMember, ProtoMember(2)]
        public string first_name { get; set; }
        [DataMember, ProtoMember(3)]
        public string last_name { get; set; }
        [DataMember, ProtoMember(4)]
        public City city { get; set; }
        [DataMember, ProtoMember(5)]
        public Country country { get; set; }
        [DataMember, ProtoMember(6)]
        public int can_post { get; set; }
        [DataMember, ProtoMember(7)]
        public int can_see_all_posts { get; set; }
        [DataMember, ProtoMember(8)]
        public string site { get; set; }
        [DataMember, ProtoMember(9)]
        public string status { get; set; }
        [DataMember, ProtoMember(10)]
        public string activity { get; set; }
        [DataMember, ProtoMember(11)]
        public int verified { get; set; }
        [DataMember, ProtoMember(12)]
        public string mobile_phone { get; set; }
        [DataMember, ProtoMember(13)]
        public string home_phone { get; set; }
    }
}
