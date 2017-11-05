using Psycho.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Psycho.Common.Domain;
using Psycho.Common.Service;
using MongoDB.Driver;
using Serilog;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;
using System.Drawing.Imaging;

namespace Psycho.Service.Implementations
{
    class QuestionnaireRepository : IQuestionnaireRepository
    {
        private IMongoDatabase _database;
        private ILogger _log;

        public QuestionnaireRepository(IMongoDatabase database, ILogger log)
        {
            _database = database;
            _log = log;

            var qcollection = _database.GetCollection<QuestionnaireScope>(nameof(QuestionnaireScope));
            long scopeCount = qcollection.Count(FilterDefinition<QuestionnaireScope>.Empty);
            //_log.Information($"Repository contains {scopeCount} scopes.");
            if (scopeCount == 0)
            {
                if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes")))
                {
                    var files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scopes"), "*.xml");
                    foreach (var fname in files)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));

                        try
                        {
                            using (var reader = new StreamReader(fname))
                            {
                                var scope = (QuestionnaireScope)serializer.Deserialize(reader);
                                scope.Capacity = scope.Entries.Count;
                                //PackScope(scope);
                                if (scope.Id == 172681828)
                                {
                                    _log.Information("fixing schwartz");
                                    foreach (var answ in scope.Entries)
                                        foreach (var a in answ.Answers)
                                            foreach (var m in a.Mappings)
                                                m.Weight = a.Score;
                                }
                                qcollection.InsertOne(scope);
                                _log.Information($"Scope {scope} stored to database");
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Information(fname);
                            _log.Error(ex, ex.Message);
                        }
                    }
                }
            }
        }

        private void PackScope(QuestionnaireScope scope)
        {
            scope.ThumbnailImage = CompressImage(scope.ThumbnailImage);
            scope.BannerImage = CompressImage(scope.BannerImage);
            foreach (var entry in scope.Entries)
                entry.QuestionImage = CompressImage(entry.QuestionImage);
            foreach (var outcoime in scope.Outcomes)
            {
                try
                {
                    outcoime.DescriptionImage = CompressImage(outcoime.DescriptionImage);
                }
                catch (Exception ex)
                {
                    _log.Error(ex, ex.Message);
                }
            }
        }

        private string CompressImage(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            var data = Convert.FromBase64String(input.Replace("data:image/jpeg;base64,", ""));
            var conve = SetQualityLevel(data, 70);
            return $"data:image/jpeg;base64,{Convert.ToBase64String(conve)}";
        }

        private byte[] SetQualityLevel(byte[] input, long quality=50l)
        {
            using (var ms = new MemoryStream(input))
            using (var oms = new MemoryStream())
            using (var img = new Bitmap(ms))
            {
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                System.Drawing.Imaging.Encoder myEncoder =
                    System.Drawing.Imaging.Encoder.Quality;

                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;
                img.Save(oms, jpgEncoder, myEncoderParameters);
                return oms.ToArray();
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public bool Add(QuestionnaireScope scope)
        {
            try
            {
                _log.Information("Assign unique id to answers...");
                var answers = scope.Entries.SelectMany(z => z.Answers);
                var count = 0;
                foreach (var answ in answers)
                    answ.Id = ++count;
                _log.Information("Store to database...");

                var qcollection = _database.GetCollection<QuestionnaireScope>(nameof(QuestionnaireScope));

                while (qcollection.Count(z => z.Id == scope.Id) != 0)
                {
                    scope.Id = Environment.TickCount;//to prevent references to removed scopes
                }
                qcollection.InsertOne(scope);
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return false;
            }
        }

        public QuestionnaireScope FindQuestionnaireById(long scopeId)
        {
            try
            {
                var qcollection = _database.GetCollection<QuestionnaireScope>(nameof(QuestionnaireScope));
                var res = qcollection.Find(z => z.Id == scopeId);
                return res.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return null;
            }
        }

        public List<DTOConciseQuestionnaireScope> GetAll()
        {
            try
            {
                var qcollection = _database.GetCollection<QuestionnaireScope>(nameof(QuestionnaireScope));

                var condition = Builders<QuestionnaireScope>.Filter.Empty;
                var fields = Builders<QuestionnaireScope>.Projection
                    .Include(p => p.Id)
                    .Include(p => p.Name)
                    .Include(p => p.ThumbnailImage)
                    .Include(p => p.Description)
                    .Include(p => p.Annotation)
                    .Include(p => p.Generation)
                    .Include(p => p.Passed);

                var results = qcollection.Find(condition).Project<DTOConciseQuestionnaireScope>(fields).ToList();

                return results;

            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return null;
            }
        }

        public bool RemoveQuestionnaire(long scopeId)
        {
            try
            {
                var qcollection = _database.GetCollection<QuestionnaireScope>(nameof(QuestionnaireScope));
                var res = qcollection.DeleteOne(z => z.Id == scopeId);
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return false;
            }
        }

        public bool Upsert(QuestionnaireScope scope)
        {
            try
            {
                var qcollection = _database.GetCollection<QuestionnaireScope>(nameof(QuestionnaireScope));
                if (qcollection.Count(z => z.Id == scope.Id) == 0)
                {
                    qcollection.InsertOne(scope);
                }
                else
                {
                    qcollection.ReplaceOne(z => z.Id == scope.Id, scope);
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return false;
            }
        }
    }
}
