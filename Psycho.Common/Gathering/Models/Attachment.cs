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
    public class Attachment
    {
        [DataMember, ProtoMember(1)]
        public string type { get; set; }
        [DataMember, ProtoMember(2)]
        public Photo photo { get; set; }
        [DataMember, ProtoMember(3)]
        public Link link { get; set; }
        [DataMember, ProtoMember(4)]
        public Market market { get; set; }
        [DataMember, ProtoMember(5)]
        public Audio audio { get; set; }
        [DataMember, ProtoMember(6)]
        public Video video { get; set; }
        [DataMember, ProtoMember(7)]
        public Note note { get; set; }
        [DataMember, ProtoMember(8)]
        public Doc doc { get; set; }
    }
}
