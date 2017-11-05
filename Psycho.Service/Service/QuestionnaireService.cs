using Psycho.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Psycho.Common.Domain;
using Psycho.Common.Domain.UserData;
using System.ServiceModel;
using System.Runtime.CompilerServices;
using Psycho.Common.Repository;
using Facebook;
using System.Net;
using System.IO;
using Serilog;
using Psycho.Service.Interfaces;
using Psycho.Common.Domain.UserData.Guts;
using System.Diagnostics;
using System.Runtime.Caching;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using Psycho.Service.Implementations;
using System.Text.RegularExpressions;
using System.ServiceModel.Web;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("Psycho.UnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Psycho.Service
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
      InstanceContextMode = InstanceContextMode.PerCall,
      IncludeExceptionDetailInFaults = true,
      MaxItemsInObjectGraph = int.MaxValue)]
    internal class QuestionnaireService : IQuestionnaireService
    {
        private IUserValidation _userValidation;
        private IRespondentsRepository _respondentsRepository;
        private ILogger _log;

        private IQuestionnaireRepository _questionnaireRepository;
        private readonly IOutcomeComputer _outcomeComputer;
        private static readonly ConcurrentDictionary<string, QuestionnaireScope> _questionnaireScopes = new ConcurrentDictionary<string, QuestionnaireScope>();
        private readonly GatheringManager<SocialNetworkDataFacebook> _gfb;
        private readonly GatheringManager<SocialNetworkDataVkontakte> _gvk;
        private static Regex _sexRegex = new Regex(@"sex\b.{0,3}(?<sex>\d{1}),");
        private static Regex _bdayRegex = new Regex(@"bdate.{1,3}\b(?<bdate>[\d\.]+)\b");
        private CongruenceOutcomeComputer _congruenceOutcomeComputer;

        public QuestionnaireService(IUserValidation userValidation,
            IRespondentsRepository respondentsRepository,
            IQuestionnaireRepository questionnaireRepository,
            GatheringManager<SocialNetworkDataFacebook> gfb,
            GatheringManager<SocialNetworkDataVkontakte> gvk,
            ILogger log,
            IOutcomeComputer outcomeComputer,
            CongruenceOutcomeComputer congruenceOutcomeComputer)
        {
            OperationContext.Current.InstanceContext.Closed += InstanceContextClosed;

            _userValidation = userValidation;
            _respondentsRepository = respondentsRepository;
            _questionnaireRepository = questionnaireRepository;
            _log = log;
            _gfb = gfb;
            _gvk = gvk;
            _outcomeComputer = outcomeComputer;
            _congruenceOutcomeComputer = congruenceOutcomeComputer;
            
        }

        private void InstanceContextClosed(object sender, EventArgs e)
        {
            //_log.Information($"Session closed");
        }

        public bool AddQuestionnaire(string token, QuestionnaireScope scope)
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();

            var res = _questionnaireRepository.Add(scope);
            _log.Information($"Operation finished with result: {res}");
            return res;
        }

        public List<DTOConciseQuestionnaireScope> GetAllQuestionnairies(string token)
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();
            ObjectCache cache = MemoryCache.Default;
            _log.Information($"GetAllQuestionnairies received");
            var scopes = _questionnaireRepository.GetAll();
            return scopes;
        }

        public string GetAuthToken(string login, string password)
        {
            _log.Information($"{GetCurrentMethod()} called.");
            return _userValidation.Auth(login, password);
        }

        public PermanentResults GetPermanentResult(string token, string permanentResultToken)
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();
            _log.Information($"{GetCurrentMethod()} called.");
            try
            {
                var permanent = _respondentsRepository.GetPermanentResults(permanentResultToken);
                return permanent;
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
            return new PermanentResults();
        }

        public QuestionnaireScope GetQuestionnaire(string token, long scopeId, int questionnumber = -1)
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();
            try
            {
                var cacheKey = $"Scope_{scopeId}";
                QuestionnaireScope scope;
                if (_questionnaireScopes.TryGetValue(cacheKey, out QuestionnaireScope tmpScope))
                    scope = CloneQuestionnaireScope(tmpScope);
                else
                {
                    _log.Information($"{GetCurrentMethod()} #{scopeId} cached.");
                    var questionaire = _questionnaireRepository.FindQuestionnaireById(scopeId);

                    if (_questionnaireScopes.TryAdd(cacheKey, questionaire))
                        scope = CloneQuestionnaireScope(questionaire);
                    else
                        scope = questionaire;
                }

                if (scope.Entries.Any(z => z.Number == questionnumber))
                {
                    scope.Entries = scope.Entries.Where(z => z.Number == questionnumber).ToList();
                    scope.Outcomes.Clear();
                }
                return scope;

            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return null;
            }
        }

        private static QuestionnaireScope CloneQuestionnaireScope(QuestionnaireScope source)
        {
            return new QuestionnaireScope
            {
                Annotation = source.Annotation,
                BannerImage = source.BannerImage,
                Capacity = source.Capacity,
                Description = source.Description,
                Entries = new List<QuestionnaireEntry>(source.Entries),
                Generation = source.Generation,
                Id = source.Id,
                Name = source.Name,
                OutcomeHeader = source.OutcomeHeader,
                OutcomeLimit = source.OutcomeLimit,
                Outcomes = new List<QuestionnaireOutcome>(source.Outcomes),
                Passed = source.Passed,
                QuestionHeader = source.QuestionHeader,
                ThumbnailImage = source.ThumbnailImage
            };
        }

        public List<string> GetRespondentHistory(string token, long respondentId)
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();
            _log.Information($"{GetCurrentMethod()} called.");
            try
            {
                var target = _respondentsRepository.FindById(respondentId);
                if (target != null)
                    return target.AttendedQuestionnairies.Select(z => z.PublicToken).ToList();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
            return new List<string>();
        }

        public List<long> GetRespondentsScopes(string token, long respondentId)
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();
            _log.Information($"{GetCurrentMethod()} called.");
            try
            {
                var target = _respondentsRepository.FindById(respondentId);
                if (target != null)
                    return target.AttendedQuestionnairies.Select(z => z.ScopeId).ToList();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
            return new List<long>();
        }

        public bool RemoveQuestionnaire(string token, long scopeId)
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();
            _log.Information($"{GetCurrentMethod()} called.");
            return _questionnaireRepository.RemoveQuestionnaire(scopeId);
        }

        public bool TraceStartTime(string token, long scopeId, long respondentId)
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();

            _log.Information($"{GetCurrentMethod()} called.");
            try
            {
                var target = _respondentsRepository.FindById(respondentId);

                target.Activity.Add(new ActivityData { Code = ActivityCode.StartQuestionaire, RelatedScopeId = scopeId });
                _respondentsRepository.Save(target);

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return false;
            }
        }

        public string UpdateRespondentQS(string token, long respondentId, QuestionnaireChoices passedQuestionnaire)
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();
            _log.Information($"{GetCurrentMethod()} called.");
            try
            {
                //lock (_locker)
                {
                    var scope = _questionnaireRepository.FindQuestionnaireById(passedQuestionnaire.ScopeId);
                    scope.Passed++;
                    _questionnaireRepository.Upsert(scope);

                    var res = _outcomeComputer.ComputeOutcomes(passedQuestionnaire, scope);

                    var permanent = new PermanentResults
                    {
                        OutcomeHeader = scope.OutcomeHeader,
                        ScopeId = passedQuestionnaire.ScopeId,
                        Outcomes = res.ToList()
                    };

                    var permatoken = _respondentsRepository.StorePermanentResults(permanent);

                    var target = _respondentsRepository.FindById(respondentId);
                    target.Activity.Add(new ActivityData { Code = ActivityCode.PassedQuestionaire, RelatedScopeId = passedQuestionnaire.ScopeId });

                    var extended = new ExtendedQuestionnaireChoices
                    {
                        AnswerIds = passedQuestionnaire.AnswerIds,
                        ScopeId = passedQuestionnaire.ScopeId,
                        PublicToken = permatoken
                    };

                    target.AttendedQuestionnairies.Add(extended);
                    _respondentsRepository.Save(target);
                    return permatoken;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return "";
            }
        }



        public long UpsertRespondentFacebook(string token, string facebookId, string fbAccessToken)
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();
            _log.Information($"{GetCurrentMethod()} called.");

            var respondent = _respondentsRepository.FindByFacebookId(facebookId);

            if (respondent == null)
            {
                respondent = _respondentsRepository.CreateNew();
                respondent.FbId = facebookId;
                _respondentsRepository.Save(respondent);
            }

            _log.Information($"[{token}] Request for fb info respId {respondent?.Id ?? -1}, {fbAccessToken}");
            _gfb.Enqueue(respondent.Id, fbAccessToken);
            return respondent.Id;


        }

        public long UpsertRespondentVkontakte(string token, int vkId, string vkAccessToken, string email = "")
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();
            _log.Information($"{GetCurrentMethod()} called.");
            var respondent = _respondentsRepository.FindByVkontakteId(vkId);

            if (respondent == null)
            {
                _log.Information("Create new user.");
                respondent = _respondentsRepository.CreateNew();
                respondent.VkId = vkId;
                respondent.Email = email;
                _respondentsRepository.Save(respondent);
            }

            _log.Information($"[{token}] Request for vk info respId {respondent?.Id ?? -1}, {vkAccessToken}");

            _gvk.Enqueue(respondent.Id, vkAccessToken);

            return respondent.Id;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        
        public Stream GetOutcomesDataset(string token, long questionaireId)
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();
            _log.Information($"{GetCurrentMethod()} called.");

            

            WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = $"attachment; filename={questionaireId}.csv";
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";
            //WebOperationContext.Current.OutgoingResponse.ContentType = "application/txt";
            var quest = questionaireId== 224053985? LoadSpecialCase() : _questionnaireRepository.FindQuestionnaireById(questionaireId);
            if (questionaireId == 224053985)
                questionaireId = 224053984;

            if (quest == null)
                return new MemoryStream();

            var resps = _respondentsRepository.FindByAttendedQuestionairesId(questionaireId);

            if (!(resps?.Any() ?? false))
                return new MemoryStream();

            var sb = new StringBuilder();
            sb.AppendLine($"UserId;VkId;FbId;Sex;bdate;elapsed;shannon;redundancy;{string.Join(";", quest.Entries.OrderBy(z => z.Number).Select(z => $"q_{z.Number}"))};{string.Join(";", quest.Outcomes.OrderBy(z => z.Index).Select(z => z.CaptionText))}");

            foreach (var user in resps)
                foreach (var session in user.AttendedQuestionnairies.Where(z => z.ScopeId == questionaireId))
                {
                    var sexMatch = _sexRegex.Match(user.DataVkontakte?.UsersGet ?? "");
                    var bdayMatch = _bdayRegex.Match(user.DataVkontakte?.UsersGet ?? "");
                    var sex = sexMatch.Success ? sexMatch.Groups["sex"].Value : "0";
                    var bday = bdayMatch.Success ? bdayMatch.Groups["bdate"].Value : "-";
                    var start = user.Activity.FirstOrDefault(z => z.Code == ActivityCode.StartQuestionaire && z.RelatedScopeId == session.ScopeId);
                    var finish = user.Activity.FirstOrDefault(z => z.Code == ActivityCode.PassedQuestionaire && z.RelatedScopeId == session.ScopeId);
                    var dimension = quest.Entries.Max(z => z.Answers.Count);
                    var shannon = ComputeShannon(session.AnswerIds,dimension);
                    var redundancy = -Math.Log(1.0 / dimension);
                    var line = $"{user.Id};{user.VkId};{user.FbId};{sex};{bday};{(finish?.Timestamp - start?.Timestamp)?.Seconds ?? 0:0};{shannon};{shannon / redundancy}";

                    foreach (var entry in quest.Entries.OrderBy(z => z.Number))
                    {
                        var choice = entry.Answers.Select(z => z.Id).Intersect(session.AnswerIds);
                        if (choice.Count() != 1)
                            line += ";-";
                        else
                        {
                            line += $";{entry.Answers.FirstOrDefault(z => choice.Contains(z.Id))?.Score}";
                        }
                    }
                    //var mps = quest.Entries.SelectMany(z => z.Answers.Where(x => session.AnswerIds.Contains(x.Id)).SelectMany(x =>  x.Mappings)).Select(z =>  z.OutcomeIndex).ToArray();
                    var mps = from a in quest.Entries
                              from b in a.Answers
                              where session.AnswerIds.Contains(b.Id)
                              from m in b.Mappings
                              select new { Score = b.Score, Mapping = m.OutcomeIndex };

                    var specline = "";
                    foreach (var oc in quest.Outcomes.OrderBy(z => z.Index))
                        specline += $";{mps.Where(z => z.Mapping == oc.Index).DefaultIfEmpty()?.Sum(z => z.Score) ?? 0}";
                    line += specline;
                    sb.AppendLine(line);
                }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            WebOperationContext.Current.OutgoingResponse.ContentLength = bytes.Length;
            return new MemoryStream(bytes);
        }

        private QuestionnaireScope LoadSpecialCase()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));

            try
            {
                using (var reader = new StreamReader("224053985.xml"))
                {
                    var scope = (QuestionnaireScope)serializer.Deserialize(reader);
                    scope.Capacity = scope.Entries.Count;
                    return scope;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
            return new QuestionnaireScope();
        }

        private double ComputeShannon(IList<int> answers, int dim)
        {
            var nest = new double[dim];
            foreach (var answ in answers)
                nest[answ % dim]++;

            var sum = nest.Sum();

            return -nest.Where(z => z != 0).Sum(z => (1.0 / z) * Math.Log(1.0 / z));
        }

        static private object _cacheLock = new object();
        public PermanentCongruenceResults GetPermanentCongruenceResult(string token, string permanentResultTokenA, string permanentResultTokenB)
        {
            if (!_userValidation.ValidateToken(token))
                throw new AddressAccessDeniedException();
            _log.Information($"{GetCurrentMethod()} called.");
            var cacheKey = $"Congruence_{permanentResultTokenA}_{permanentResultTokenB}";
            PermanentCongruenceResults retval;

            if (MemoryCache.Default.Contains(cacheKey))
            {
                lock (_cacheLock)
                {
                    retval = MemoryCache.Default[cacheKey] as PermanentCongruenceResults;
                }
            }
            else
            {
                retval = _congruenceOutcomeComputer.ComputeOutcome(permanentResultTokenA, permanentResultTokenB);

                lock (_cacheLock)
                {
                    CacheItemPolicy cip = new CacheItemPolicy()
                    {
                        AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(120))
                    };

                    MemoryCache.Default.Set(cacheKey, retval, cip);
                }
            }
            
            return retval;
        }
    }
}
