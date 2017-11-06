using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Service.Messages
{
    [ProtoContract]
    public class MessageWallPostLikesRepostsComments
    {
        /// <summary>
        ///     Со знаком минус, если айдишник принадлежит группе
        /// </summary>
        [ProtoMember(1)]
        public int OwnerId { get; set; }

        [ProtoMember(2)]
        public int WallPostId { get; set; }

        [ProtoMember(3)]
        public string PostType { get; set; }
    }
}
