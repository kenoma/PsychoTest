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
    public class WallResponse
    {
        [ProtoMember(1), DataMember]
        public int count { get; set; }
        [ProtoMember(2), DataMember]
        public List<WallPost> items { get; set; }
        [ProtoMember(3), DataMember]
        public List<Profile> profiles { get; set; }
        [ProtoMember(4), DataMember]
        public List<GroupData> groups { get; set; }
        [ProtoMember(5), DataMember]
        public int GroupId { get; set; }
    }
}
