using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Domain.UserData.Guts
{
    public class ExtendedQuestionnaireChoices: QuestionnaireChoices
    {
        [BsonId]
        private ObjectId _id = ObjectId.GenerateNewId();

        public string PublicToken { get; set; }
    }
}
