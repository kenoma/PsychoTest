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
    public class Doc
    {
        [DataMember, ProtoMember(1)]
        public int id { get; set; }
        [DataMember, ProtoMember(2)]
        public int owner_id { get; set; }
        [DataMember, ProtoMember(3)]
        public string title { get; set; }
        [DataMember, ProtoMember(4)]
        public int size { get; set; }
        [DataMember, ProtoMember(5)]
        public string ext { get; set; }
        //[DataMember, ProtoMember(6)]
        //public string url { get; set; }
        [DataMember, ProtoMember(6)]
        public int date { get; set; }
        [DataMember, ProtoMember(7)]
        public int type { get; set; }
        [DataMember, ProtoMember(8)]
        public string access_key { get; set; }
    }
}
