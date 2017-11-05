using Newtonsoft.Json;
using Psycho.Gathering.Interfaces;
using Psycho.Gathering.Models;
using Psycho.Validator.helpers;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XGBoost;

namespace Psycho.Gathering.Implementations
{
    class Manager
    {
        private readonly ILogger _log;
        private readonly IReadOnlyList<UserProfileGathering> _laborer;
        private readonly IReadOnlyList<GroupGathering> _groupLaborers;
        private readonly IUserGetRepository _repo;
        private readonly IWallPostRepository _wallRepo;
        private readonly IAntiBotRepository _antiBotRepository;
        private double _average = 30000.0;
        public bool IsBusy { get; private set; }
        private int _lastQueueCount = 0;
        readonly private Dictionary<long, double[]> _groupmapping;

        public Manager(IReadOnlyList<UserProfileGathering> userGathering,
            IReadOnlyList<GroupGathering> groupLaborers, 
            ILogger log, 
            IUserGetRepository repo,
            IWallPostRepository wallRepo, 
            IAntiBotRepository antiBotRepository)
        {
            _groupmapping = JsonConvert.DeserializeObject<Dictionary<long, double[]>>(File.ReadAllText("map_groups.json"));
            _antiBotRepository = antiBotRepository;
            _laborer = userGathering;
            _groupLaborers = groupLaborers;
            _log = log;
            _repo = repo;
            _wallRepo = wallRepo;
            _log.Information($"Gathering can be performed with {_laborer.Count} laborers");
        }

        public void StartGathering(ConcurrentQueue<int> queue)
        {
            IsBusy = true;
            
            var threads = new List<Thread>();
            
            var timer = new Timer(PrintStatistics, queue, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            _log.Information("Start gathering {QueueCount} items.", queue.Count);
            _lastQueueCount = queue.Count;

            var slice = queue.Count / _laborer.Count;

            foreach (var lab in _laborer)
            {
                var thread = new Thread(() => UserProfileLabor(lab, queue))
                {
                    IsBackground = true
                };
                threads.Add(thread);
                thread.Start();
            }

            foreach (var t in threads)
                t.Join(1000 * 60 * 60 * 4);

            foreach (var t in threads)
                if (t.IsAlive)
                    t.Abort();

            IsBusy = false;
        }

        
        private void PrintStatistics(object state)
        {
            var queue = state as ConcurrentQueue<int>;
            int actualSize = queue.Count;
            double delta = _lastQueueCount - actualSize;
            _lastQueueCount = actualSize;
            _log.Information("Remains [{QueueSize}] Alive: {AliveGath} {AverageGatheringTime:0} ms. Days by average: {DaysRemains:0.0} days By actual speed: {DaysBySpeed:0.0} days. Delta: {Delta}",
                actualSize,
                _laborer.Count(z => !z.IsSpoiled),
                _average,
                TimeSpan.FromMilliseconds(actualSize * _average / _laborer.Count(z => !z.IsSpoiled)).TotalDays,
                (actualSize / delta) / (24.0 * 60.0), delta);
        }

        private void UserProfileLabor(UserProfileGathering gath, ConcurrentQueue<int> queue)
        {
            try
            {
                
                using (var xgbc = BaseXgbModel.LoadClassifierFromFile("ext_trained_model.xgb"))
                {
                    xgbc.SetParameter("num_class", 2);
                    _log.Verbose("Thread {GathId} is ready to go!", gath.Id);
                    var users = new List<UserGet>();
                    while (queue.Count != 0 && !gath.IsSpoiled)
                    {
                        while (queue.TryDequeue(out int id))
                        {
                            var sw = Stopwatch.StartNew();
                            var userdata = gath.RetrieveUserData(id);
                            gath.Processed++;
                            _average = 0.999 * _average + 0.001 * sw.ElapsedMilliseconds;
                            _log.Verbose("thread id {GathId} VkId: {VkId} takes {WholeProfileElapsedTime} ms. Gathered {GatheresItemCount}", gath.Id, id, sw.ElapsedMilliseconds, gath.Processed);

                            if (userdata == null)
                            {
                                continue;
                            }
                            users.Add(userdata);
                            if (users.Count > 50)
                            {
                                DetectBotsAndSave(gath, _groupmapping, xgbc, users);
                                users.Clear();
                            }
                        }
                    }

                    DetectBotsAndSave(gath, _groupmapping, xgbc, users);

                    if (queue.Count != 0)
                        _log.Error("Job unfinished, but laborer {GathId} died. Gathered {GatheresItemCount}", gath.Id, gath.Processed);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
            //_log.Debug("Thread finished");
        }

        private void DetectBotsAndSave(UserProfileGathering gath, Dictionary<long, double[]> groupmapping, XGBClassifier xgbc, List<UserGet> users)
        {
            var vectors = users.Select(z => z.ToVector(groupmapping)).ToArray();
            var preds = xgbc.Predict(vectors);
            _log.Information("Thread {GathId} intended to save {NoBotsUserCount}/{UserCount} accounts", gath.Id, preds.Count(z => z == 0), users.Count);

            var tmp = new List<UserGet>();
            var isbot = new List<Tuple<int, bool>>();
            for (int usr = 0; usr < users.Count; usr++)
            {
                isbot.Add(new Tuple<int, bool>(users[usr].id, preds[usr] == 1));
                if (preds[usr] == 0)
                    tmp.Add(users[usr]);
            }

            if (isbot.Any())
                _antiBotRepository.SaveUsers(isbot);

            if (tmp.Any())
                _repo.SaveUsers(tmp, DateTime.Now);
        }

        async internal Task StartGatheringGroupsAsync()
        {
            IsBusy = true;
            await Task.Run(() =>
            {
                var files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "groupraw"));

                foreach (var file in files)
                {
                    _log.Information($"Processing {file}");
                    var rnd = new Random();
                    var listId = File.ReadAllLines(file).OrderBy(z => rnd.NextDouble()).Distinct().ToArray();
                    double remains = listId.Length;

                    Parallel.ForEach(listId, new ParallelOptions { MaxDegreeOfParallelism = _groupLaborers.Count }, sid =>
                    {
                        if (int.TryParse(sid, out int id))
                        {
                            try
                            {
                                GroupGathering gath = null;
                                while (_groupLaborers.Any(z => !z.IsSpoiled) && gath == null)
                                {
                                    var tmp = _groupLaborers[rnd.Next(_groupLaborers.Count)];
                                    if (!tmp.IsSpoiled)
                                        gath = tmp;
                                }

                                if (gath == null)
                                {
                                    _log.Warning($"Gathering stopped due to lack of alive tokens");
                                    return;
                                }

                                var sw = Stopwatch.StartNew();
                                var wallData = gath.FillWallInfo(id);

                                if (!_wallRepo.SaveWallpost(wallData))
                                    _log.Warning($"Failed to save group {id}");
                                else
                                    _log.Information($"[{remains-- / listId.Length:0.000}] {_groupLaborers.Count(z => !z.IsSpoiled)} -{id} {sw.ElapsedMilliseconds}ms");

                            }
                            catch (Exception ex)
                            {
                                _log.Error(ex, ex.Message);
                            }
                        }
                    });
                }
            });
            IsBusy = false;
        }
    }
}
