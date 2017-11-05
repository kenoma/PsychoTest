using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Domain.AdminArea
{
    public class ServiceUser : IAggregateRoot
    {
        [BsonId]
        public ObjectId _id = ObjectId.GenerateNewId();

        public string Username { get; set; }
        public string Password { get; set; }

        public long Id { get; internal set; } = Environment.TickCount;
    }
}
