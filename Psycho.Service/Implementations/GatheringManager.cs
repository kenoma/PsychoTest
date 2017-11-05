using Serilog;
using Psycho.Common.Domain.UserData;
using Psycho.Common.Domain.UserData.Guts;
using Psycho.Common.Repository;
using Psycho.Service.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Psycho.Service.Implementations
{
    class GatheringManager<T> where T : class
    {
        private ISocialNetworkGrabber<T> _grabber;
        private IRespondentsRepository _respondentsRepository;
        private ILogger _log;
        private ConcurrentQueue<Tuple<long, string>> _queue = new ConcurrentQueue<Tuple<long, string>>();
        private Thread _thread;
        private AutoResetEvent _holder = new AutoResetEvent(false);
        private bool _isAlive = true;

        public GatheringManager(ISocialNetworkGrabber<T> grabber,
                        IRespondentsRepository respondentsRepository,
                        ILogger log)
        {
            _grabber = grabber;
            _respondentsRepository = respondentsRepository;
            _log = log;
            _thread = new Thread(StartGathering);
            _thread.Start();
        }

        private void StartGathering()
        {
            while (_isAlive)
            {
                _holder.WaitOne();
                while (!_queue.IsEmpty && _queue.TryDequeue(out Tuple<long, string> tpl))
                {
                    _log.Information($"Queue contains {_queue.Count} records.");
                    try
                    {
                        var userData = _grabber.GetUserData(tpl.Item2);
                        SaveData(userData, tpl.Item1);
                        _log.Information("User data obtained and saved.");
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, ex.Message);
                    }
                }
            }
        }

        private void SaveData<M>(M data, long respId)
        {
            try
            {
                var target = _respondentsRepository.FindById(respId);
                Type itemType = typeof(T);
                if (itemType == typeof(SocialNetworkDataVkontakte))
                {
                    target.DataVkontakte = data as SocialNetworkDataVkontakte;
                    _log.Information($"Save VK {respId}");
                }
                if (itemType == typeof(SocialNetworkDataFacebook))
                {
                    target.DataFacebook = data as SocialNetworkDataFacebook;
                    _log.Information($"Save FB {respId}");
                }

                target.Activity.Add(new ActivityData { Code = ActivityCode.SocialNetworkDataLoaded });
                _respondentsRepository.Save(target);
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
        }

        internal void Enqueue(long respondentId, string token)
        {
            if (!_queue.Any(z => z.Item1 == respondentId && z.Item2 == token))
            {
                _log.Information($"User {respondentId} | {token} enqueued to {_queue.Count} others");
                _queue.Enqueue(new Tuple<long, string>(respondentId, token));
                _holder.Set();
            }
            else
            {
                _log.Information($"User {respondentId} already enslaved.");
            }
        }
    }
}
