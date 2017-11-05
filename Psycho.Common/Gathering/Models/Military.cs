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
    public class Military : ILocalAggregateRoot
    {
        [DataMember, ProtoMember(1)]
        public string unit { get; set; }
        [DataMember, ProtoMember(2)]
        public int unit_id { get; set; }
        [DataMember, ProtoMember(3)]
        public int country_id { get; set; }

        [IgnoreDataMember, ProtoIgnore]
        public int id { get; set; }
    }
}
