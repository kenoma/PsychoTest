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
    public class Likes
    {
        [DataMember, ProtoMember(1)]
        public ushort count { get; set; }
        [DataMember, ProtoMember(2)]
        public ushort user_likes { get; set; }
        [DataMember, ProtoMember(3)]
        public byte can_like { get; set; }
        [DataMember, ProtoMember(4)]
        public byte can_publish { get; set; }
    }
}
