using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Service.Messages
{
    [ProtoContract]
    public class MessageUserGet
    {
        [ProtoMember(1)]
        public int UserGetId { get; set; }
    }
}
