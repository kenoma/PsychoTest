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
    public class Link
    {
        [DataMember, ProtoMember(1)]
        public string url { get; set; }
        [DataMember, ProtoMember(2)]
        public string title { get; set; }
        [DataMember, ProtoMember(3)]
        public string caption { get; set; }
        [DataMember, ProtoMember(4)]
        public string description { get; set; }
        [DataMember, ProtoMember(5)]
        public byte is_external { get; set; }
        [DataMember, ProtoMember(6)]
        public Photo photo { get; set; }
    }
}
