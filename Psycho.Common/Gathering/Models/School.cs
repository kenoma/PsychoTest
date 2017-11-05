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
    public class School
    {
        [DataMember, ProtoMember(1)]
        public string id { get; set; }
        [DataMember, ProtoMember(2)]
        public int country { get; set; }
        [DataMember, ProtoMember(3)]
        public int city { get; set; }
        [DataMember, ProtoMember(4)]
        public string name { get; set; }
        [DataMember, ProtoMember(5)]
        public string @class { get; set; }
        [DataMember, ProtoMember(6)]
        public string speciality { get; set; }
    }
}
