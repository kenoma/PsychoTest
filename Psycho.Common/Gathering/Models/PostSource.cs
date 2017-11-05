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
    public class PostSource
    {
        [DataMember, ProtoMember(1)]
        public string type { get; set; }
        [DataMember, ProtoMember(2)]
        public string platform { get; set; }
        [DataMember, ProtoMember(3)]
        public string data { get; set; }
        [DataMember, ProtoMember(4)]
        public Link link { get; set; }
    }

}
