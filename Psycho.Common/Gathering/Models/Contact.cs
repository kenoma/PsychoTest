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
    public class Contact
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        [DataMember, ProtoMember(1)]
        public int user_id { get; set; }

        /// <summary>
        /// Должность.
        /// </summary>
        [DataMember, ProtoMember(2)]
        public string desc { get; set; }

        /// <summary>
        /// Электронная почта.
        /// </summary>
        [DataMember, ProtoMember(3)]
        public string email { get; set; }

        /// <summary>
        /// Телефон.
        /// </summary>
        [DataMember, ProtoMember(4)]
        public string phone { get; set; }

    }
}
