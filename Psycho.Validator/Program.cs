using AutoMapper;
using CsvHelper;
using MongoDB.Driver;
using Newtonsoft.Json;
using OfficeOpenXml;
using PokerMind.DataMining;
using PokerMind.stuff;
using Psycho.Common.Domain;
using Psycho.Common.Domain.UserData;
using Psycho.Gathering.Implementations;
using Psycho.Gathering.Model;
using Psycho.Gathering.Models;
using Psycho.Validator.helpers;
using Psycho.Validator.models;
using Psycho.Validator.models.train;
using Serilog;
using SharpLearning.Containers.Matrices;
using SharpLearning.RandomForest.Learners;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XGBoost;

namespace Psycho.Validator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter command:");
            var cmd = Console.ReadLine();
            switch (cmd)
            {
                case "flat":
                    FlatternUsers();
                    break;
                case "goup_class":
                    ExtractGroupInfo();
                    break;
                case "antibot":
                    AntiBot();
                    break;
                case "repack":
                    Repack();
                    break;
                case "reveal":
                    Reveal();
                    break;
                case "quest_fifth":
                    QuestionaireDatasetPreparation.StoreDatasetFifth();
                    break;
                case "train_fifth":
                    Fifth();
                    break;
                case "compute_fifth":
                    ComputeFifth();
                    break;
                case "clean":
                    Clean();
                    break;
                case "katya":
                    Katya();
                    break;
                case "peter":
                    Peter();
                    break;
                case "denis":
                    Denis();
                    break;
                case "task":
                    ComputeTask();
                    break;

            }
        }

        private static void ComputeTask()
        {
            Console.Write("enter_database:");
            var dbfile = Console.ReadLine();

            var log = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.LiterateConsole()
               .CreateLogger();
            var repository = new UserGetRepository(dbfile, log, new CompressorProto());
            var publics = new HashSet<int>(File.ReadLines("task\\Publics.txt").Select(z => int.TryParse(z, out int vkid) ? vkid : -1));
            var target = new HashSet<int>(File.ReadLines("task\\532k_Min_1_opp_public_from_our_4.7kk.txt").Select(z => int.TryParse(z, out int vkid) ? vkid : -1));

            var users = new UserGet[0];
            var count = 0;
            const int batch = 50000;
            if (!File.Exists("task.csv"))
                File.WriteAllText($"task.csv", "VkId\tGroups\r\n");
            do
            {
                try
                {
                    users = repository.RangeSelect(count, batch).ToArray();
                    foreach (var user in users)
                    {
                        if (target.Contains(user.id))
                        {
                            var common = publics.Intersect(user.Groups?.Select(z => z.id) ?? new int[0]);
                            foreach (var c in common)
                                File.AppendAllText("task.csv", $"{user.id}\t{c}\r\n");
                            log.Information("Done with {UserId}", user.id);
                        }
                    }
                    count += batch;
                    ///log.Information("Done {Count} recs.", count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            } while (users.Length != 0);
            log.Information("Done");
        }

        private static void Denis()
        {
            Console.Write("Enter dataset:");
            var cString = Console.ReadLine();
            Console.Write("Enter path to h_agg_not_bots:");
            var fname = Console.ReadLine();

            var repo = new DenisResultsRepository(cString);
            repo.CleanAll();

            var data = new List<DenisModel>();
            var csv = new CsvReader(File.OpenText(fname), new CsvHelper.Configuration.CsvConfiguration { CultureInfo = CultureInfo.InvariantCulture });
            while (csv.Read())
            {
                data.Add(csv.GetRecord<DenisModel>());
            }
            repo.Insert(data);
            Console.WriteLine("'{0}' done");

            Console.WriteLine("done");
        }

        private static void Peter()
        {
            Console.Write("Enter dataset:");
            var cString = Console.ReadLine();
            Console.Write("Enter dir:");
            var dir = Console.ReadLine();
            var files = DirSearch(dir);
            var repo = new PeterResultsRepository(cString);
            repo.CleanAll();

            foreach (var f in files)
            {
                var data = new List<PeterModel>();
                var csv = new CsvReader(File.OpenText(f), new CsvHelper.Configuration.CsvConfiguration { CultureInfo = CultureInfo.InvariantCulture });
                while (csv.Read())
                {
                    data.Add(csv.GetRecord<PeterModel>());
                }
                repo.Insert(data);
                Console.WriteLine("'{0}' done");
            }
            Console.WriteLine("done");
        }

        private static void Katya()
        {
            Console.Write("Enter dataset:");
            var cString = Console.ReadLine();
            Console.Write("Enter dir:");
            var dir = Console.ReadLine();
            var files = DirSearch(dir);
            var repo = new NLPResultsRepository(cString);
            repo.CleanAll();

            foreach (var f in files)
            {
                var vkids = new List<int>();
                var vals = new List<double[]>();
                var csv = new CsvReader(File.OpenText(f), new CsvHelper.Configuration.CsvConfiguration { CultureInfo = CultureInfo.InvariantCulture });
                //var headers = csv.FieldHeaders.Except(new string[] { "user" }).ToArray();
                while (csv.Read())
                {
                    vkids.Add(csv.GetField<int>("user"));
                    var tmp = new double[58];
                    for (int i = 0; i < 58; i++)
                        tmp[i] = csv.GetField<double?>(i + 1) ?? -1;
                    vals.Add(tmp);
                }
                repo.Insert(vkids.ToArray(), vals.Select(z => z.Select(x => (float)x).ToArray()).ToArray());
                Console.WriteLine("'{0}' done");
            }
            Console.WriteLine("done");
        }

        static private List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir, "*.csv"))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt);
            }

            return files;
        }

        private static void Clean()
        {
            var log = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.LiterateConsole()
                    .CreateLogger();

            try
            {
                Console.Write("Enter antibot:");
                var antibot = Console.ReadLine();
                var regex = new Regex(@"(?<id>\d+)\b,(?<isbot>\d)");
                var bots = File.ReadAllLines(antibot).Skip(1).Select(z => regex.Match(z)).Where(z => z.Success && z.Groups["isbot"].Value == "1").Select(z => int.Parse(z.Groups["id"].Value)).ToArray();

                Console.Write("Enter dataset:");
                var cString = Console.ReadLine();


                var repository = new UserGetRepository(cString, log, new CompressorProto());

                repository.DeleteUsers(bots);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            Console.WriteLine("Done");
        }

        private static void ComputeFifth()
        {
            var fifther = new Fifther();
            fifther.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fifth"));
            
            Console.Write("Enter dataset:");
            var cString = Console.ReadLine();
            var bagOfTerms = File.ReadAllLines("res\\expert_topics.csv").Skip(1).Select(z => z.Split(',')).ToDictionary(z => int.Parse(z[0]), z => new HashSet<string>(z[1].Split(' ')));

            var scope = new QuestionnaireScope();
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            using (var reader = new StreamReader("test_fifth.xml"))
            {
                scope = (QuestionnaireScope)serializer.Deserialize(reader);
            }

            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.LiterateConsole()
                .CreateLogger();
            var repository = new UserGetRepository(cString, log, new CompressorProto());
            var fifthRepo = new FifthResultRepository(cString);
            var groupmapping = JsonConvert.DeserializeObject<Dictionary<long, double[]>>(File.ReadAllText("map_groups.json"));
            fifthRepo.CleanAll();
            var count = 0;
            var gsw = Stopwatch.StartNew();
            UserGet[] users = new UserGet[0];
            const int batch = 1000;
            do
            {
                try
                {
                    users = repository.RangeSelect(count, batch).ToArray();
                    users = users.Where(z => z?.Groups?.Count > 0).ToArray();
                                        
                    float[][] input = new float[users.Length][];
                    for (int user = 0; user < users.Length; user++)
                    {
                        input[user] = users[user].ToVector(groupmapping, bagOfTerms);
                    }
                    var preds = fifther.PredictDistr(input, 5);

                    fifthRepo.Insert(users.Select(z => z.id).ToArray(), preds.Select(z => QuestionaireDatasetPreparation.PredictionsToScales(scope, z)).ToArray());
                   
                    count += batch;
                    log.Information("Done {Count} recs. {DaysForMillion} days", count, TimeSpan.FromMilliseconds(1000000 * gsw.ElapsedMilliseconds / count).TotalDays);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            } while (users.Length != 0);
            
            Console.WriteLine("Done");
            Console.ReadLine();
            Console.ReadLine();
        }

        private static Fifther Fifth()
        {
            var groupmapping = JsonConvert.DeserializeObject<Dictionary<long, double[]>>(File.ReadAllText("map_groups.json"));
            var bagOfTerms = File.ReadAllLines("res\\expert_topics.csv").Skip(1).Select(z => z.Split(',')).ToDictionary(z => int.Parse(z[0]), z => new HashSet<string>(z[1].Split(' ')));
            var xFilename = "224053984_dataset.json";
            
            var scope = new QuestionnaireScope();
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireScope));
            using (var reader = new StreamReader("test_fifth.xml"))
            {
                scope = (QuestionnaireScope)serializer.Deserialize(reader);
            }

            var log = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.LiterateConsole()
               .CreateLogger();
            var repository = new UserGetRepository("passed_tests_ferrets.s3db", log, new CompressorProto());
            Dictionary<int, float[]> vectors = new Dictionary<int, float[]>();
            var users = new UserGet[0];
            var count = 0;
            const int batch = 1000;
            do
            {
                try
                {
                    users = repository.RangeSelect(count, batch).ToArray();
                    foreach (var user in users)
                    {
                        vectors.Add(user.id, user.ToVector(groupmapping, bagOfTerms));
                    }
                    count += batch;
                    log.Information("Done {Count} recs.", count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            } while (users.Length != 0);

            //File.WriteAllText("datax.csv", string.Join(", ", FlatUsertToVectorMapping.GetHeader(groupmapping.FirstOrDefault().Value.Length, bagOfTerms.Count)) + "\r\n");
            //File.AppendAllLines("datax.csv", vectors.Select(z => string.Join(", ", z.Value.Select(x => x.ToString("0.000", CultureInfo.InvariantCulture)))));

            var X = JsonConvert.DeserializeObject<List<FifthAttendance>>(File.ReadAllText(xFilename)).Where(z => vectors.ContainsKey(z.vkid)).Where(z => QuestionaireDatasetPreparation.CovertAnswersToVector(scope, z.AnswersId).Length == 120).ToArray();
            var Y = X.Select(z => QuestionaireDatasetPreparation.CovertAnswersToVector(scope, z.AnswersId)).ToArray();
            File.WriteAllLines("answers.csv", Y.Select(z => string.Join(", ", z)));


            var rnd = new Random(Environment.TickCount);
            var train = Enumerable.Range(0, X.Length).OrderBy(z => rnd.NextDouble()).ToArray();

            var x_train = train.Take(X.Length * 80 / 100).Select(z => vectors[X[z].vkid]).ToArray();
            var y_train = train.Take(X.Length * 80 / 100).Select(z => Y[z]).ToArray();
            var x_test = train.Skip(X.Length * 80 / 100).Select(z => vectors[X[z].vkid]).ToArray();
            var y_test = train.Skip(X.Length * 80 / 100).Select(z => Y[z]).ToArray();

            var fifther = new Fifther();
            Console.WriteLine();

            for (int qnum = 0; qnum < 120; qnum++)
            {
                Console.WriteLine($"Question: {qnum}");
                var yds = y_train.Select(z => (float)z[qnum]).ToArray();
                var ytds = y_test.Select(z => (float)z[qnum]).ToArray();

                var parameters = new Dictionary<string, object>();
                parameters["max_depth"] = 10;
                parameters["learning_rate"] = 0.1f;
                parameters["n_estimators"] = 300;
                parameters["silent"] = true;
                parameters["objective"] = "multi:softprob";//"binary:logistic";//

                parameters["nthread"] = -1;
                parameters["gamma"] = 4f;
                parameters["min_child_weight"] = 2;
                parameters["max_delta_step"] = 1;
                parameters["subsample"] = 1f;
                parameters["colsample_bytree"] = 1f;
                parameters["colsample_bylevel"] = 1f;
                parameters["reg_alpha"] = 0f;
                parameters["reg_lambda"] = 1f;
                parameters["scale_pos_weight"] = 1f;

                parameters["base_score"] = 0.8F;
                parameters["seed"] = 0;
                parameters["missing"] = float.NaN;
                parameters["num_class"] = 5;
                var xgbc = new XGBClassifier(parameters);
                xgbc.Fit(x_train, yds);

                fifther.AddLevel(qnum, xgbc);

                var discrepancy = 0.0;
                var dist = 0.0;
                var preds = xgbc.PredictDistr(x_train);

                for (int pos = 0; pos < preds.Length; pos++)
                {
                    var tmp = new float[5];
                    tmp[(int)yds[pos]] = 1f;
                    dist += Math.Abs(det.GetMaxIndex(preds[pos]) - yds[pos]);
                    discrepancy += det.EuclidianDistance(tmp, preds[pos]);
                }
                Console.WriteLine("[Train] Discrepancy {0:0.000} Dist {1:0.000}", 1.0 - discrepancy / (preds.Length * Math.Sqrt(2.0)), dist / preds.Length);
                preds = xgbc.PredictDistr(x_test);
                discrepancy = 0.0;
                dist = 0.0;
                for (int pos = 0; pos < preds.Length; pos++)
                {
                    var tmp = new float[5];
                    tmp[(int)yds[pos]] = 1f;
                    dist += Math.Abs(det.GetMaxIndex(preds[pos]) - yds[pos]);
                    discrepancy += det.EuclidianDistance(tmp, preds[pos]);
                }
                Console.WriteLine("[Test ] Discrepancy {0:0.000} Dist {1:0.000}", 1.0 - discrepancy / (preds.Length * Math.Sqrt(2.0)), dist / preds.Length);
            }
            Console.WriteLine("Done");
            fifther.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fifth"));
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            return fifther;
        }

        private static void Reveal()
        {
            Console.Write("Enter dataset:");
            var cString = Console.ReadLine();

            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.LiterateConsole()
                .CreateLogger();
            var repository = new UserGetRepository(cString, log, new CompressorProto());
            var groupmapping = JsonConvert.DeserializeObject<Dictionary<long, double[]>>(File.ReadAllText("map_groups.json"));
            var xgbc = BaseXgbModel.LoadClassifierFromFile("ext_trained_model.xgb");
            xgbc.SetParameter("num_class", 2);
            var count = 0;
            var sb = new StringBuilder();
            sb.AppendLine("VkId,IsBot");
            var gsw = Stopwatch.StartNew();
            UserGet[] users = new UserGet[0];
            do
            {
                try
                {
                    users = repository.RangeSelect(count, 10000).ToArray();
                    float[][] input = new float[users.Length][];
                    for (int user = 0; user < users.Length; user++)
                    {
                        input[user] = users[user].ToVector(groupmapping);
                    }
                    var preds = xgbc.Predict(input);
                    for (int user = 0; user < users.Length; user++)
                    {
                        sb.AppendLine($"{users[user].id},{preds[user]}");
                    }
                    count += 10000;
                    File.WriteAllText($"IsBot_{cString}.csv", sb.ToString());
                    log.Information("Done {Count} recs. Bpc {BotPercent}, {DaysForMillion} days", count, preds.Sum() / preds.Length, TimeSpan.FromMilliseconds(1000000 * gsw.ElapsedMilliseconds / count).TotalDays);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            } while (users.Length != 0);
            File.WriteAllText($"IsBot_{cString}.csv", sb.ToString());
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void Repack()
        {
            Console.Write("Enter dataset:");
            var cString = Console.ReadLine();
            Console.Write("Enter repacket dataset:");
            var target = Console.ReadLine();
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.LiterateConsole()
                .CreateLogger();
            var repository = new UserGetRepository(cString, log, new CompressorLZ4());
            var targetRepository = new UserGetRepository(target, log, new CompressorProto());

            var count = 0;
            var users = new List<UserGet>();
            var gsw = Stopwatch.StartNew();
            do
            {
                users = new List<UserGet>(repository.RangeSelect(count, 1000));
                count += 1000;
                targetRepository.SaveUsers(users, DateTime.Now);

                log.Information("Done {Count} recs. {DaysForMillion} days", count, TimeSpan.FromMilliseconds(1000000 * gsw.ElapsedMilliseconds / count).TotalDays);
            } while (users.Count != 0);
        }

        private static void AntiBot()
        {
            //var X = JsonConvert.DeserializeObject<List<float[]>>(File.ReadAllText("X.json"));
            //var Y = JsonConvert.DeserializeObject<List<float>>(File.ReadAllText("Y.json"));

            var X = new List<float[]>();
            var Y = new List<float>();

            var pt1 = ReadRepo("nopack_nobot_ferrets.s3db", 0.0f);
            var pt2 = ReadRepo("nopack_bots_ferrets.s3db", 1.0f);
            X.AddRange(pt1.x);
            X.AddRange(pt2.x);
            Y.AddRange(pt1.y);
            Y.AddRange(pt2.y);

            File.WriteAllText("antibot_xy.csv", "Y," + string.Join(", ", FlatUsertToVectorMapping.GetHeader(0, 0)) + ", " + string.Join(", ", FlatUsertToVectorMapping.GetHeader(0, 0)) + ", " + string.Join(", ", FlatUsertToVectorMapping.GetHeader(0, 0)) +", "+ string.Join(", ", FlatUsertToVectorMapping.GetHeader(102, 0)) + "\r\n");
            File.AppendAllLines("antibot_xy.csv", X.Select((z, i) => (int)Y[i] + ", " + string.Join(", ", z.Select(x => x.ToString(CultureInfo.InvariantCulture)))));
            File.WriteAllText("antibot_x.csv", string.Join(", ", FlatUsertToVectorMapping.GetHeader(0, 0)) + ", " + string.Join(", ", FlatUsertToVectorMapping.GetHeader(0, 0)) + ", " + string.Join(", ", FlatUsertToVectorMapping.GetHeader(0, 0)) + ", " + string.Join(", ", FlatUsertToVectorMapping.GetHeader(102, 0)) + "\r\n");
            File.AppendAllLines("antibot_x.csv", X.Select((z, i) => string.Join(", ", z.Select(x => x.ToString(CultureInfo.InvariantCulture)))));

            //File.WriteAllText("X.json", JsonConvert.SerializeObject(X));
            //File.WriteAllText("Y.json", JsonConvert.SerializeObject(Y));

            var rnd = new Random(Environment.TickCount);
            var train = Enumerable.Range(0, X.Count).OrderBy(z => rnd.NextDouble()).ToArray();

            var x_train = train.Take(X.Count * 80 / 100).Select(z => X[z]).ToArray();
            var y_train = train.Take(X.Count * 80 / 100).Select(z => Y[z]).ToArray();
            var x_test = train.Skip(X.Count * 80 / 100).Select(z => X[z]).ToArray();
            var y_test = train.Skip(X.Count * 80 / 100).Select(z => Y[z]).ToArray();

            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine($"Age;Sex");
            //for (int pos = 0; pos < X.Count; pos++)
            //    if (Y[pos] == 0)
            //    {
            //        sb.AppendLine($"{X[pos][0]};{X[pos][19]}");
            //    }
            //File.WriteAllText("socdem.csv", sb.ToString());

            var parameters = new Dictionary<string, object>();
            parameters["max_depth"] = 10;
            parameters["learning_rate"] = 0.1f;
            parameters["n_estimators"] = 500;
            parameters["silent"] = true;
            parameters["objective"] = "multi:softprob";//"binary:logistic";//

            parameters["nthread"] = -1;
            parameters["gamma"] = 0f;
            parameters["min_child_weight"] = 1;
            parameters["max_delta_step"] = 1;
            parameters["subsample"] = 1f;
            parameters["colsample_bytree"] = 1f;
            parameters["colsample_bylevel"] = 1f;
            parameters["reg_alpha"] = 1.5f;
            parameters["reg_lambda"] = 1f;
            parameters["scale_pos_weight"] = 1f;

            parameters["base_score"] = 0.5F;
            parameters["seed"] = 0;
            parameters["missing"] = float.NaN;
            parameters["num_class"] = 2;
            using (var txgbc = new XGBClassifier(parameters))
            {
                txgbc.Fit(x_train, y_train);
                txgbc.SaveModelToFile("ext_trained_model.xgb");
            }

            using (var xgbc = BaseXgbModel.LoadClassifierFromFile("ext_trained_model.xgb"))
            {
                xgbc.SetParameter("num_class", 2);
                var testDiscrepancy = 0.0;
                var trainDiscrepancy = 0.0;
                var preds = xgbc.Predict(x_test);
                for (int pos = 0; pos < preds.Length; pos++)
                    testDiscrepancy += Math.Abs(y_test[pos] - preds[pos]) < 1e-3 ? 1 : 0;
                testDiscrepancy /= preds.Length;

                preds = xgbc.Predict(x_train);
                for (int pos = 0; pos < preds.Length; pos++)
                    trainDiscrepancy += Math.Abs(y_train[pos] - preds[pos]) < 1e-3 ? 1 : 0;
                trainDiscrepancy /= preds.Length;
                Console.WriteLine("Train/Test Quality {0}/{1}", trainDiscrepancy, testDiscrepancy);
            }
            Console.ReadKey();
        }

        private static (float[][] x, float[] y) ReadRepo(string cString, float label)
        {
            var repository = new UserGetRepository(cString, new LoggerConfiguration()
                            .MinimumLevel.Verbose()
                            .WriteTo.LiterateConsole()
                            .CreateLogger(), new CompressorProto());
            var groupmapping = JsonConvert.DeserializeObject<Dictionary<long, double[]>>(File.ReadAllText("map_groups.json"));
            //var bagOfTerms = File.ReadAllLines("res\\expert_topics.csv").Skip(1).Select(z => z.Split(',')).ToDictionary(z => int.Parse(z[0]), z => new HashSet<string>(z[1].Split(' ')));

            var count = 0;
            var users = new List<UserGet>();

            var sbx = new List<float[]>();
            var sby = new List<float>();

            do
            {
                users = new List<UserGet>(repository.RangeSelect(count, 1000));
                count += 1000;
                foreach (var user in users)
                {
                    var vector = user.ToVector(groupmapping /*bagOfTerms*/);
                    sbx.Add(vector);
                    sby.Add(label);
                }
                Console.WriteLine("{0}: {1} read", cString, count);
            } while (users.Count != 0);

            return (sbx.ToArray(), sby.ToArray());
        }

        private static void ExtractGroupInfo()
        {
            Console.Write("Enter group xlsx:");
            var cString = Console.ReadLine();
            var categories = new Dictionary<string, int>();
            var mapping = new Dictionary<long, double[]>();

            using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(cString)))
            {
                var wsheet = xlPackage.Workbook.Worksheets.First();
                var totalRows = wsheet.Dimension.End.Row;
                var totalColumns = wsheet.Dimension.End.Column;

                for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                {
                    try
                    {
                        var id = wsheet.Cells[rowNum, 1].Value;
                        var cats = wsheet.Cells[rowNum, 5, rowNum, 5 + 7].Where(z => z.Value.ToString() != "0").Select(z => z.Value.ToString()).ToArray();
                        foreach (var c in cats)
                        {
                            var spl = c.Split(';').Select(z => z.Trim());
                            foreach (var s in spl)
                                if (!categories.ContainsKey(s))
                                    categories.Add(s, categories.Count);
                        }
                        Console.WriteLine($"{id} categories listed");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                {
                    try
                    {
                        var id = int.Parse(wsheet.Cells[rowNum, 1].Value.ToString());
                        var cats = wsheet.Cells[rowNum, 5, rowNum, 5 + 7].Where(z => z.Value.ToString() != "0").Select(z => z.Value.ToString()).ToArray();
                        var outcome = new double[categories.Count];
                        foreach (var c in cats)
                        {
                            var spl = c.Split(';').Select(z => z.Trim());
                            foreach (var s in spl)
                                outcome[categories[s]] = 1;
                        }
                        mapping.Add(id, outcome);
                        Console.WriteLine($"{id} group mapped");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }

            File.WriteAllText("map_categories.json", JsonConvert.SerializeObject(categories));
            File.WriteAllText("map_groups.json", JsonConvert.SerializeObject(mapping));
        }

        private static void FlatternUsers()
        {
            //Console.Write("Enter connection string:");
            //var cString = Console.ReadLine();
            //var client = new MongoClient(cString);
            //var db = client.GetDatabase("psychodb");
            //var rcollection = db.GetCollection<RespondentUser>("RespondentUser");
            //var flatcollection = db.GetCollection<FlatUser>("FlatUsers");
            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<UserGet, FlatUser>()
            //    .ForMember(z => z.nickname, z => z.MapFrom(x => string.IsNullOrEmpty(x.nickname) ? 0 : 1))
            //    .ForMember(z => z.twitter, z => z.MapFrom(x => string.IsNullOrEmpty(x.twitter) ? 0 : 1))
            //    .ForMember(z => z.skype, z => z.MapFrom(x => string.IsNullOrEmpty(x.skype) ? 0 : 1))
            //    .ForMember(z => z.maiden_name, z => z.MapFrom(x => string.IsNullOrEmpty(x.maiden_name) ? 0 : 1))
            //    .ForMember(z => z.career, z => z.MapFrom(x => (x.career != null && x.career.Any()) ? x.career.Count : 0))
            //    .ForMember(z => z.military, z => z.MapFrom(x => (x.military != null && x.military.Any()) ? x.military.Count : 0))
            //    .ForMember(z => z.schools, z => z.MapFrom(x => (x.schools != null && x.schools.Any()) ? x.schools.Count : 0))
            //    .ForMember(z => z.universities, z => z.MapFrom(x => (x.universities != null && x.universities.Any()) ? x.universities.Count : 0))
            //    .ForMember(z => z.relatives, z => z.MapFrom(x => (x.relatives != null && x.relatives.Any()) ? x.relatives.Count : 0))
            //    .ForMember(z => z.age, z => z.MapFrom(x => Age(x.bdate)));
            //});

            //var users = rcollection.Find(z => z.DataVkontakte != null);
            //using (var cursor = users.ToCursor())
            //{
            //    while (cursor.MoveNext())
            //    {
            //        foreach (var user in cursor.Current)

            //            try
            //            {
            //                var duser = JsonConvert.DeserializeObject<RootObjectUsderGet>(user.DataVkontakte.UsersGet);
            //                var req2 = JsonConvert.DeserializeObject<RootObjectTwo>(user.DataVkontakte.RequestOne);
            //                if (duser?.response?.Any() ?? false)
            //                {
            //                    var trainuser = Mapper.Map<FlatUser>(duser.response[0]);
            //                    trainuser.followers = Mapper.Map<IList<FlatUser>>(req2.response.req_0.items).ToArray();
            //                    trainuser.friends = Mapper.Map<IList<FlatUser>>(req2.response.req_3.items).ToArray();
            //                    trainuser.groups = req2.response.req_5.items.ToArray();
            //                    flatcollection.InsertOne(trainuser);
            //                    Console.WriteLine($"User {trainuser.id} done");
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine($"User failed");
            //            }
            //    }
            //}
        }





    }

}
