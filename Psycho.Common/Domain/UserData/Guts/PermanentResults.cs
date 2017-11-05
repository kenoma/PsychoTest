using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Domain.UserData.Guts
{
    public class PermanentResults
    {
        [BsonId]
        private ObjectId _id = ObjectId.GenerateNewId();

        public string OutcomeHeader { get; set; } = "";

        public string PublicToken { get; set; } = ObjectId.GenerateNewId().ToString();

        public long ScopeId { get; set; } = Environment.TickCount;

        public IList<QuestionnaireOutcome> Outcomes { get; set; }
    }
}
