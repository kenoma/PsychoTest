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
    public class Personal
    {
        [DataMember, ProtoMember(1)]
        public string religion { get; set; }
        [DataMember, ProtoMember(2)]
        public string inspired_by { get; set; }
        [DataMember, ProtoMember(3)]
        public int life_main { get; set; }
        [DataMember, ProtoMember(4)]
        public int smoking { get; set; }
        [DataMember, ProtoMember(5)]
        public int alcohol { get; set; }
        [DataMember, ProtoMember(6)]
        public int political { get; set; }
    }
}
