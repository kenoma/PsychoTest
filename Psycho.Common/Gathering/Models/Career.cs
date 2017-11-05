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
    public class Career
    {
        [DataMember, ProtoMember(1)]
        public int group_id { get; set; }
        [DataMember, ProtoMember(2)]
        public int country_id { get; set; }
        [DataMember, ProtoMember(3)]
        public int city_id { get; set; }
    }
}
