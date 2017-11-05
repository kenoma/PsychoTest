using ProtoBuf;
using Psycho.Common.Repository.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Models
{
    [ProtoContract, DataContract]
    public class Occupation: ILocalAggregateRoot
    {
        [DataMember, ProtoMember(1)]
        public string type { get; set; }

        [DataMember, ProtoMember(2)]
        public int id { get; set; }

        [DataMember, ProtoMember(3)]
        public string name { get; set; }
    }
}
