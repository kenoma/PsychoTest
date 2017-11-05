using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Psycho.Common.Domain.UserData.Guts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Domain.UserData
{
    public class RespondentUser : IAggregateRoot
    {
        [BsonId]
        private ObjectId _id = ObjectId.GenerateNewId();

        public long Id { get; internal set; } = Environment.TickCount;

        public int VkId { get; set; }

        public string FbId { get; set; }

        public string Email { get; set; }

        public SocialNetworkDataVkontakte DataVkontakte { get; set; }

        public SocialNetworkDataFacebook DataFacebook { get; set; }

        public List<ExtendedQuestionnaireChoices> AttendedQuestionnairies { get; set; } = new List<ExtendedQuestionnaireChoices>();

        public List<ActivityData> Activity { get; set; } = new List<ActivityData>();
    }
}
