using Psycho.Common.Domain.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Psycho.Common.Domain;
using Psycho.Common.Domain.UserData.Guts;

namespace Psycho.Common.Repository
{
    public interface IRespondentsRepository
    {
        RespondentUser FindById(long id);
        RespondentUser FindByVkontakteId(int vkId);
        RespondentUser FindByFacebookId(string facebookId);
        IList<RespondentUser> FindByAttendedQuestionairesId(long questId);

        void Save(RespondentUser respondent);

        RespondentUser CreateNew();

        PermanentResults GetPermanentResults(string permatoken);
        RespondentUser GetUserByToken(string permatoken);

        string StorePermanentResults(PermanentResults permanentResults);
    }
}
