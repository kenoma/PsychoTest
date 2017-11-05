using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Psycho.Common.Domain;
using Psycho.Common.Service;

namespace Psycho.Common.Repository
{
    public interface IQuestionnaireRepository
    {
        List<DTOConciseQuestionnaireScope> GetAll();
        QuestionnaireScope FindQuestionnaireById(long scopeId);
        bool RemoveQuestionnaire(long scopeId);
        bool Add(QuestionnaireScope scope);
        bool Upsert(QuestionnaireScope scope);
    }
}
