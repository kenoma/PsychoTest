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
    public class Price
    {
        [DataMember, ProtoMember(1)]
        public string amount { get; set; }
        [DataMember, ProtoMember(2)]
        public Currency currency { get; set; }
        [DataMember, ProtoMember(3)]
        public string text { get; set; }
    }
}
