using Psycho.Common.Domain;
using Psycho.Common.Domain.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Service.Interfaces
{
    interface IOutcomeComputer
    {
        List<QuestionnaireOutcome> ComputeOutcomes(QuestionnaireChoices passedQuestionnaire, QuestionnaireScope scope);
    }
}
