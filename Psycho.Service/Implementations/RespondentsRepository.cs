using Psycho.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Psycho.Common.Domain.UserData;
using MongoDB.Driver;
using Serilog;
using Psycho.Common.Domain;
using Psycho.Common.Domain.UserData.Guts;
using Newtonsoft.Json;
using Serilog.Context;
using System.IO;

namespace Psycho.Service.Implementations
{
    class RespondentsRepository : IRespondentsRepository
    {
        private IMongoDatabase _database;
        private ILogger _log;

        public RespondentsRepository(IMongoDatabase database, ILogger log)
        {
            _database = database;
            _log = log;
            _log.Information("Init user repo");

            
        }

        public RespondentUser CreateNew()
        {
            try
            {
                var rcollection = _database.GetCollection<RespondentUser>(nameof(RespondentUser));
                var neophyte = new RespondentUser();

                while (rcollection.Count(z => z.Id == neophyte.Id) != 0)
                {
                    neophyte.Id = Environment.TickCount;
                }
                rcollection.InsertOne(neophyte);
                _log.Information($"New user created {neophyte}");
                return neophyte;
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return null;
            }
        }

        public IList<RespondentUser> FindByAttendedQuestionairesId(long scopeId)
        {
            try
            {
                var rcollection = _database.GetCollection<RespondentUser>(nameof(RespondentUser));

                var fields = Builders<RespondentUser>.Projection
                   .Include(p => p.Id)
                   .Include(p => p.VkId)
                   .Include(p => p.DataVkontakte.UsersGet)
                   .Include(p => p.Activity)
                   .Include(p => p.AttendedQuestionnairies);

                return rcollection.Find(z => z.AttendedQuestionnairies.Any(x=>x.ScopeId==scopeId)).Project<RespondentUser>(fields).ToList();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return null;
            }
        }

        public RespondentUser FindByFacebookId(string facebookId)
        {
            try
            {
                var rcollection = _database.GetCollection<RespondentUser>(nameof(RespondentUser));
                var cursor = rcollection.Find(z=>z.FbId == facebookId);
                return cursor.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return null;
            }
        }

        public RespondentUser FindById(long id)
        {
            try
            {
                var rcollection = _database.GetCollection<RespondentUser>(nameof(RespondentUser));
                var cursor = rcollection.Find(z => z.Id == id).Limit(1);
                return cursor.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return null;
            }
        }

        public RespondentUser FindByVkontakteId(int vkId)
        {
            try
            {
                var rcollection = _database.GetCollection<RespondentUser>(nameof(RespondentUser));

                var cursor = rcollection.Find(z => z.VkId == vkId).Limit(1);
                return cursor.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return null;
            }
        }

        public RespondentUser GetUserByToken(string permatoken)
        {
            try
            {
                var rcollection = _database.GetCollection<RespondentUser>(nameof(RespondentUser));
             
                return rcollection.Find(z => z.AttendedQuestionnairies.Any(x => x.PublicToken == permatoken))
                    .SingleOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
            return new RespondentUser();
        }

        public PermanentResults GetPermanentResults(string permalink)
        {
            try
            {
                var collection = _database.GetCollection<PermanentResults>(nameof(PermanentResults));
                var cursor = collection.Find(z => z.PublicToken == permalink).Limit(1);
                var res = cursor.FirstOrDefault();
                if (res != null)
                    return res;
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
            return new PermanentResults();
        }

        public void Save(RespondentUser respondent)
        {
            try
            {
                var rcollection = _database.GetCollection<RespondentUser>(nameof(RespondentUser));
                if (rcollection.Count(z => z.Id == respondent.Id) == 0)
                {
                    rcollection.InsertOne(respondent);
                }
                else
                {
                    rcollection.ReplaceOne(z => z.Id == respondent.Id, respondent);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
        }

        public string StorePermanentResults(PermanentResults permanentResults)
        {
            try
            {
                var pcollection = _database.GetCollection<PermanentResults>(nameof(PermanentResults));
                pcollection.InsertOne(permanentResults);
                return permanentResults.PublicToken;
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
            return "";
        }
    }
}
