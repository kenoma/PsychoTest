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
    public class CopyHistory
    {
        [DataMember, ProtoMember(1)]
        public int id { get; set; }
        [DataMember, ProtoMember(2)]
        public int owner_id { get; set; }
        [DataMember, ProtoMember(3)]
        public int from_id { get; set; }
        [DataMember, ProtoMember(4)]
        public int date { get; set; }
        [DataMember, ProtoMember(5)]
        public string post_type { get; set; }
        [DataMember, ProtoMember(6)]
        public string text { get; set; }
        [DataMember, ProtoMember(7)]
        public List<Attachment> attachments { get; set; }
        [DataMember, ProtoMember(8)]
        public PostSource post_source { get; set; }
        [DataMember, ProtoMember(9)]
        public int signer_id { get; set; }
    }
}
