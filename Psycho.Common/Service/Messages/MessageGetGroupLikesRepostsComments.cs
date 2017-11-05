using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Service.Messages
{
    [ProtoContract]
    public class MessageGetGroupLikesRepostsComments
    {
        [ProtoMember(1)]
        public int GroupId { get; set; }

        [ProtoMember(2)]
        public int PostCount { get; set; }
    }
}
