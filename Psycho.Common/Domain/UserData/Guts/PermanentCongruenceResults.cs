using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Domain.UserData.Guts
{
    public class PermanentCongruenceResults
    {
        [BsonId]
        private ObjectId _id = ObjectId.GenerateNewId();

        public long ScopeId { get; set; } = Environment.TickCount;

        public double MagicCoefficient { get; set; } = 0;

        public IList<CongruenceOutcome> CongruenceOutcomes { get; set; }

        public double RecommendationRate { get; set; }

        public string RecommendationText { get; set; }

        public IList<string> UserNames { get; set; }
        public IList<string> UserAvatarts { get; set; }
        public IList<int> UserIds { get; set; }
    }
}
