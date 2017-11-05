using AutoMapper;
using MongoDB.Driver;
using Newtonsoft.Json;
using Psycho.Common.Domain;
using Psycho.Common.Domain.UserData;
using Psycho.Gathering.Models;
using Psycho.Service.Implementations;
using Psycho.Validator.helpers;
using Psycho.Validator.models;
using Psycho.Validator.models.train;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Validator
{
    class QuestionaireDatasetPreparation
    {
        private class RootObjectUsderGet
        {
            public List<UserGet> response { get; set; }
        }


        static public void StoreDatasetFifth()
        {
            Console.Write("Enter string:");
            var cString = Console.ReadLine();
            var client = new MongoClient(cString);
            var db = client.GetDatabase("psychodb");
            var rcollection = db.GetCollection<RespondentUser>(nameof(RespondentUser));
            var qcollection = db.GetCollection<QuestionnaireScope>(nameof(QuestionnaireScope));
            const int fifthId = 224053984;
            var quest = qcollection.Find(z => z.Id == fifthId).FirstOrDefault();
            //var groupmapping = JsonConvert.DeserializeObject<Dictionary<long, double[]>>(File.ReadAllText("map_groups.json"));
            var users = rcollection.Find(z => z.DataVkontakte != null && z.AttendedQuestionnairies.Any(x => x.ScopeId == fifthId));

            var xdataset = new List<FifthAttendance>();
            
            using (var cursor = users.ToCursor())
            {
                while (cursor.MoveNext())
                {
                    foreach (var user in cursor.Current)

                        try
                        {
                            var duser = JsonConvert.DeserializeObject<RootObjectUsderGet>(user.DataVkontakte.UsersGet);
                            var req2 = JsonConvert.DeserializeObject<RootObjectTwo>(user.DataVkontakte.RequestOne);
                            if (duser?.response?.Any() ?? false)
                            {
                                var trainuser = duser.response[0];
                                //trainuser.followers = Mapper.Map<IList<FlatUser>>(req2.response.req_0.items).ToArray();
                                //trainuser.friends = Mapper.Map<IList<FlatUser>>(req2.response.req_3.items).ToArray();
                                trainuser.Groups = req2.response.req_5.items.ToList();

                                if (user.AttendedQuestionnairies.Any(z => z.ScopeId == fifthId))
                                {
                                    foreach (var attend in user.AttendedQuestionnairies.Where(z => z.ScopeId == fifthId))
                                    {
                                        //xdataset.Add(trainuser.ToVector(groupmapping));
                                        xdataset.Add(new FifthAttendance { vkid = trainuser.id, AnswersId = attend.AnswerIds.ToArray() });
                                        Console.WriteLine($"User {trainuser.id} done. Groups: {trainuser.Groups.Count}.");
                                    }        
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"User failed");
                        }
                }
            }
            File.WriteAllText("passed_tests.txt", string.Join("\r\n", xdataset.Select(z => z.vkid)));
            File.WriteAllText($"{fifthId}_dataset.json", JsonConvert.SerializeObject(xdataset));
            Console.WriteLine("Task done");
        }

        public static float[] PredictionsToScales(QuestionnaireScope scope, float[][] choices)
        {
            if (choices.Length != 120)
                throw new InvalidOperationException("Incorrent input");
            var scales = new float[5];
            try
            {
                
                for (int q = 0; q < choices.Length; q++)
                {
                    var entry = scope.Entries.FirstOrDefault(z => z.Number == q + 1);
                    if (entry != null)
                    {
                        for (int answ = 0; answ < entry.Answers.Count; answ++)
                        {
                            foreach (var map in entry.Answers[answ].Mappings)
                            {
                                scales[map.OutcomeIndex - 1] += choices[q][answ] * (entry.Answers[answ].Score + 2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return scales;
        }

        public static float[] GetScales(QuestionnaireScope scope, int[] choices)
        {
            if (choices.Length != 120)
                throw new InvalidOperationException("Incorrent input");

            var scales = new float[5];
            try
            {

                var raw = from a in scope.Entries
                          from b in a.Answers
                          from c in b.Mappings
                          let outcome = scope.Outcomes.Find(z => z.Index == c.OutcomeIndex)
                          select new
                          {
                              Key = outcome.Index-1,
                              Flag = choices.Contains(b.Id) ? (b.Score + 3) : 0.0f
                          };
                foreach (var r in raw)
                {
                    if (r.Flag != 0)
                        scales[r.Key] += r.Flag - 1.0f;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return scales;
        }

        public static double[] CovertAnswersToVector(QuestionnaireScope scope, int[] choices)
        {
            //var res = outcomeComputer.ComputeOutcomes(choices, scope);
            //var retval = new float[scope.Outcomes.Count];
            //foreach (var r in res)
            //{
            //    retval[r.Index - 1] = 1;
            //}

            var raw = from a in scope.Entries
                      from b in a.Answers
                      from c in b.Mappings
                      let outcome = scope.Outcomes.Find(z => z.Index == c.OutcomeIndex)
                      select new
                      {
                          Key = outcome.CaptionText,
                          A = b.Id,
                          Flag = choices.Contains(b.Id) ? (double)(b.Score + 3) : 0.0
                      };

            var retval = from a in raw
                         orderby a.A
                         where a.Flag != 0
                         select a.Flag - 1;
            if (retval.Count() != 120)
            {
                Console.WriteLine("haba");
            }

            //var raw = from a in scope.Entries
            //          from b in a.Answers
            //          from c in b.Mappings
            //          let outcome = scope.Outcomes.Find(z => z.Index == c.OutcomeIndex)
            //          select new
            //          {
            //              Key = outcome.CaptionText,
            //              Flag = choices.Contains(b.Id) ? (double)b.Score : 0.0
            //          };

            //var retval = from a in raw
            //             group a by a.Key into g
            //             orderby g.Key
            //             select g.Sum(z => z.Flag) / 3;

            return retval.ToArray();
        }
    }
}
