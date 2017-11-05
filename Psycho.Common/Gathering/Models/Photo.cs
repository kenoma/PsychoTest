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
    public class Photo
    {
        [DataMember, ProtoMember(1)]
        public int id { get; set; }
        [DataMember, ProtoMember(2)]
        public int album_id { get; set; }
        [DataMember, ProtoMember(3)]
        public int owner_id { get; set; }
        [DataMember, ProtoMember(4)]
        public string text { get; set; }
        [DataMember, ProtoMember(5)]
        public int date { get; set; }
        [DataMember, ProtoMember(6)]
        public int post_id { get; set; }
        [DataMember, ProtoMember(7)]
        public string access_key { get; set; }
    }
}
