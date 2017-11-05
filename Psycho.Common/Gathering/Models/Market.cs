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
    public class Market
    {
        [DataMember, ProtoMember(1)]
        public int id { get; set; }
        [DataMember, ProtoMember(2)]
        public int owner_id { get; set; }
        [DataMember, ProtoMember(3)]
        public string title { get; set; }
        [DataMember, ProtoMember(4)]
        public string description { get; set; }
        [DataMember, ProtoMember(5)]
        public Price price { get; set; }
        [DataMember, ProtoMember(6)]
        public Category category { get; set; }
        [DataMember, ProtoMember(7)]
        public int date { get; set; }
        [DataMember, ProtoMember(8)]
        public int availability { get; set; }
    }
}
