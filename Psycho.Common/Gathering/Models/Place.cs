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
    public class Place
    {
        [DataMember, ProtoMember(1)]
        public int id { get; set; }
        [DataMember, ProtoMember(2)]
        public string title { get; set; }
        [DataMember, ProtoMember(3)]
        public double latitude { get; set; }
        [DataMember, ProtoMember(4)]
        public double longitude { get; set; }
        [DataMember, ProtoMember(5)]
        public int created { get; set; }
        [DataMember, ProtoMember(6)]
        public string icon { get; set; }
        [DataMember, ProtoMember(7)]
        public int group_id { get; set; }
        [DataMember, ProtoMember(8)]
        public string group_photo { get; set; }
        [DataMember, ProtoMember(9)]
        public int checkins { get; set; }
        [DataMember, ProtoMember(10)]
        public int updated { get; set; }
        [DataMember, ProtoMember(11)]
        public int type { get; set; }
        [DataMember, ProtoMember(12)]
        public int country { get; set; }
        [DataMember, ProtoMember(13)]
        public int city { get; set; }
        [DataMember, ProtoMember(14)]
        public string address { get; set; }
    }
}
