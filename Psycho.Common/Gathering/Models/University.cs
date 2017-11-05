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
    public class University : ILocalAggregateRoot
    {
        [DataMember, ProtoMember(1)]
        public int id { get; set; }
        [DataMember, ProtoMember(2)]
        public int country { get; set; }
        [DataMember, ProtoMember(3)]
        public int city { get; set; }
        [DataMember, ProtoMember(4)]
        public string name { get; set; }
        [DataMember, ProtoMember(5)]
        public int faculty { get; set; }
        [DataMember, ProtoMember(6)]
        public string faculty_name { get; set; }
        [DataMember, ProtoMember(7)]
        public int chair { get; set; }
        [DataMember, ProtoMember(8)]
        public string chair_name { get; set; }
        [DataMember, ProtoMember(9)]
        public string education_form { get; set; }
        [DataMember, ProtoMember(10)]
        public string education_status { get; set; }
    }

}
