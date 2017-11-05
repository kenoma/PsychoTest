using Psycho.Common.Domain;
using Psycho.Common.Domain.UserData;
using Psycho.Common.Domain.UserData.Guts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Service
{
    [ServiceContract]
    public interface IQuestionnaireService
    {
        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "POST",
            UriTemplate = "/GetAuthToken?login={login}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Выдает пользователю токен для дальнейшей работы с сервисом")]
        string GetAuthToken(string login, string password);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/GetConciseQuestionnairies?token={token}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Выдает список всех опросников в сокращенной форме")]
        List<DTOConciseQuestionnaireScope> GetAllQuestionnairies(string token);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/GetQuestionnaire?token={token}&scopeId={scopeId}&questionnumber={questionnumber}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Возвращает полный опросник с указанным идентификатором")]
        QuestionnaireScope GetQuestionnaire(string token, long scopeId,int questionnumber=-1);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/TraceStartTime?token={token}&scopeId={scopeId}&respondentId={respondentId}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Возвращает полный опросник с указанным идентификатором")]
        bool TraceStartTime(string token, long scopeId, long respondentId);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "POST",
            UriTemplate = "/AddQuestionnaire?token={token}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Добавляет новый опросник в базу")]
        bool AddQuestionnaire(string token, QuestionnaireScope newScope);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/RemoveQuestionnaire?token={token}&scopeId={scopeId}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Удаляет опросник с идентификатором из базы")]
        bool RemoveQuestionnaire(string token, long scopeId);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/UpsertRespondent/VK?token={token}&vkId={vkId}&vkAccessToken={vkAccessToken}&email={email}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Создает нового пользователя вконтакте с заданным идентификатором или обновляет существуюшего. Возвращает идентификатор пользователя в базе.")]
        long UpsertRespondentVkontakte(string token, int vkId, string vkAccessToken, string email="");

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/UpsertRespondent/FB?token={token}&fbAccessToken={fbAccessToken}&facebookId={facebookId}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Создает нового пользователя фейсбука с заданным идентификатором или обновляет существуюшего. Возвращает идентификатор пользователя в базе.")]
        long UpsertRespondentFacebook(string token, string facebookId, string fbAccessToken);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "POST",
            UriTemplate = "/UpsertRespondent/QS?token={token}&respondentId={respondentId}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Обновляет информацию о прохождении теста для пользователя с указанным идентификатором. В PassedQuestionnaire передается объект теста, содержащий только ответы пользователя. Возвращает публичный идентификатор для метода GetPermanentResult")]
        string UpdateRespondentQS(string token, long respondentId, QuestionnaireChoices passedQuestionnaire);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/GetPermanentResults?token={token}&permanentResultToken={permanentResultToken}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Возвращает результат прохождения теста пользователем по его публичному идентификатору.")]
        PermanentResults GetPermanentResult(string token, string permanentResultToken);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/GetPermanentCongruenceResult?token={token}&permanentResultTokenA={permanentResultTokenA}&permanentResultTokenB={permanentResultTokenB}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Возвращает результат прохождения теста пользователем по его публичному идентификатору.")]
        PermanentCongruenceResults GetPermanentCongruenceResult(string token, string permanentResultTokenA, string permanentResultTokenB);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/GetRespondentHistory?token={token}&respondentId={respondentId}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Возвращает список публичных токенов по конкретному пользователю.")]
        List<string> GetRespondentHistory(string token, long respondentId);

        [OperationContract(IsOneWay = false),
          WebInvoke(
            Method = "GET",
            UriTemplate = "/GetRespondentsScopes?token={token}&respondentId={respondentId}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json),
          Description("Возвращает список идентификаторов тестов пройденных пользоватем.")]
        List<long> GetRespondentsScopes(string token, long respondentId);

        [OperationContract(IsOneWay = false),
          WebGet(UriTemplate = "/GetOutcomesDataset?token={token}&questionaireId={questionaireId}"),
          Description("Возвращает датасет ответов на вопросы")]
        Stream GetOutcomesDataset(string token, long questionaireId);
    }
}
