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
    public class Comments
    {
        [DataMember, ProtoMember(1)]
        public int count { get; set; }
        [DataMember, ProtoMember(2)]
        public int can_post { get; set; }
    }
}
