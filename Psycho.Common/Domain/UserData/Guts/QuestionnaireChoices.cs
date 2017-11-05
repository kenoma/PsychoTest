using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Domain.UserData
{
    public class QuestionnaireChoices
    {
        public long ScopeId { get; set; }

        public IList<int> AnswerIds { get; set; }

        public override string ToString() => $"{ScopeId}: {string.Join(", ", AnswerIds)}";
    }
}
