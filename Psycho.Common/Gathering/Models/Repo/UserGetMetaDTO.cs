using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Models.Repo
{
    [DataContract, ProtoContract]
    public class UserGetMetaDTO
    {
        [ProtoMember(1), DataMember]
        public int VkontakteUserId { get; set; }

        [ProtoMember(2), DataMember]
        public DateTime TimeStamp { get; set; }
    }
}
