using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Service.Messages
{
    [ProtoContract]
    public class MessageExtractWallPostsCommand
    {
        /// <summary>
        ///     Со знаком минус, если айдишник принадлежит группе
        /// </summary>
        [ProtoMember(1)]
        public int OwnerId { get; set; }

        /// <summary>
        /// НЕ МЕНЬШЕ 100
        /// </summary>
        [ProtoMember(2)]
        public int PostCount { get; set; }
    }
}
