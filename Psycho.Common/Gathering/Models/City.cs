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
    public class City : ILocalAggregateRoot
    {
        [DataMember, ProtoMember(1)]
        public int id { get; set; }

        //[DataMember, ProtoMember(2)]
        //public string title { get; set; }
    }
}
