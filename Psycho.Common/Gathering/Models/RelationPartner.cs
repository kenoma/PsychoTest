﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Models
{
    [ProtoContract, DataContract]
    public class RelationPartner
    {
        [DataMember, ProtoMember(1)]
        public int id { get; set; }
        [DataMember, ProtoMember(2)]
        public string first_name { get; set; }
        [DataMember, ProtoMember(3)]
        public string last_name { get; set; }
    }
}
