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
    public class StatusAudio
    {
        [DataMember, ProtoMember(1)]
        public int id { get; set; }
        [DataMember, ProtoMember(2)]
        public int owner_id { get; set; }
        [DataMember, ProtoMember(3)]
        public string artist { get; set; }
        [DataMember, ProtoMember(4)]
        public string title { get; set; }
        [DataMember, ProtoMember(5)]
        public int duration { get; set; }
        [DataMember, ProtoMember(6)]
        public int date { get; set; }
        [DataMember, ProtoMember(7)]
        public int album_id { get; set; }
        [DataMember, ProtoMember(8)]
        public int genre_id { get; set; }
    }
}
