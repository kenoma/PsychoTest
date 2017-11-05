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
    public class GroupData
    {
        [DataMember, ProtoMember(1)]
        public int id { get; set; }
        [DataMember, ProtoMember(2)]
        public string name { get; set; }
        [DataMember, ProtoMember(3)]
        public string screen_name { get; set; }
        [DataMember, ProtoMember(4)]
        public byte is_closed { get; set; }
        [DataMember, ProtoMember(5)]
        public string type { get; set; }
        [DataMember, ProtoMember(6)]
        public string description { get; set; }
        [DataMember, ProtoMember(7)]
        public string wiki_page { get; set; }
        [DataMember, ProtoMember(8)]
        public int members_count { get; set; }
        [DataMember, ProtoMember(9)]
        public int start_date { get; set; }
        [DataMember, ProtoMember(10)]
        public byte can_post { get; set; }
        [DataMember, ProtoMember(11)]
        public byte can_see_all_posts { get; set; }
        [DataMember, ProtoMember(12)]
        public string activity { get; set; }
        [DataMember, ProtoMember(13)]
        public string status { get; set; }
        [DataMember, ProtoMember(14)]
        public List<Contact> contacts { get; set; }
        [DataMember, ProtoMember(15)]
        public int fixed_post { get; set; }
        [DataMember, ProtoMember(16)]
        public byte verified { get; set; }
        [DataMember, ProtoMember(17)]
        public string site { get; set; }
        [DataMember, ProtoMember(18)]
        public byte can_create_topic { get; set; }
        [DataMember, ProtoMember(19)]
        public City city { get; set; }
        [DataMember, ProtoMember(20)]
        public Country country { get; set; }
        [DataMember, ProtoMember(21)]
        public Place place { get; set; }
        [DataMember, ProtoMember(22)]
        public string deactivated { get; set; }
    }
}
