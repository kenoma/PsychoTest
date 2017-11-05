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
    public class LastSeen
    {
        [DataMember, ProtoMember(1)]
        public int time { get; set; }
        [DataMember, ProtoMember(2)]
        public int platform { get; set; }
    }
}
