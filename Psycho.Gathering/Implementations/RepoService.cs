using Psycho.Gathering.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Psycho.Gathering.Models;
using Psycho.Gathering.Models.Repo;
using System.ServiceModel;
using Serilog;
using System.IO;

namespace Psycho.Gathering.Implementations
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single,
     InstanceContextMode = InstanceContextMode.PerCall,
     IncludeExceptionDetailInFaults = true,
     MaxItemsInObjectGraph = int.MaxValue)]
    class RepoService : IRepoService
    {
        private readonly IUserGetRepository _userGetRepository;
        private readonly ILogger _log;
        private readonly Manager _manager;
        private readonly  IWallPostRepository _wallRepo;

        public RepoService(IUserGetRepository userGetRepository, IWallPostRepository wallRepo, ILogger log, Manager manager)
        {
            _userGetRepository = userGetRepository;
            _log = log;
            _manager = manager;
            _wallRepo = wallRepo;
        }

        public UserGet[] GetUserData(string token, int id)
        {
            if (token != "OtQRZmKpl3hpiHXYEys8")
                return null;

            var retval = _userGetRepository.GetUser(id);
            return retval.ToArray();
        }

        public bool InitiateGathering(string token)
        {
            if (token != "ifihadaheart")
                return false;
            if (_manager.IsBusy)
                return false;
            _log.Warning("Deprecated!!!!");
            //_manager.StartGathering();
                //.ContinueWith(z => _log.Information("Task finished"));
            return true;
        }

        public bool InitiateGatheringGroupWalls(string token)
        {
            if (token != "ifihadaheart")
                return false;
            if (_manager.IsBusy)
                return false;

            //try
            //{
            //    _log.Information("Start filling groups");
            //    var grouipIds = new HashSet<int>();
            //    for (int i = 0; i < 900000; i += 10000)
            //    {
            //        var users = _userGetRepository.RangeSelect(i, i + 10000);
            //        foreach (var gid in users.Where(z => z.Groups != null).SelectMany(z => z.Groups.Select(x => x.id)).ToArray())
            //        {
            //            if (!grouipIds.Contains(gid))
            //                grouipIds.Add(gid);
            //        }
            //        _log.Information("Gathered data about {UniqueId} groupid", grouipIds.Count);
            //        File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"groups_{i}.txt"), grouipIds.Select(z => z.ToString()));
            //    }

            //    File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "groups.txt"), grouipIds.Select(z => z.ToString()));
            //}
            //catch (Exception ex)
            //{
            //    _log.Error(ex, ex.Message);
            //}
            _manager.StartGatheringGroupsAsync()
                .ContinueWith(z => _log.Information("Task finished"));
            return true;
        }

        public UserGetMetaDTO[] ListProfiles(string token)
        {
            if (token != "OtQRZmKpl3hpiHXYEys8")
                return null;

            _log.Information($"ListProfiles called.");
            return _userGetRepository.ListRecords().ToArray();
        }

        public WallResponse[] RangeGroupSelect(string token, int skip, int take)
        {
            if (token != "OtQRZmKpl3hpiHXYEys8")
                return null;

            _log.Information($"RangeGroupSelect called.");
            return _wallRepo.RangeSelect(skip, take).ToArray();
        }

        public byte[][] RangeRawSelect(string token, int skip, int take)
        {
            if (token != "OtQRZmKpl3hpiHXYEys8")
                return null;

            _log.Information($"ListProfiles called.");
            return _userGetRepository.RangeRawSelect(skip, take).Select(z => z.Item2).ToArray();
        }

        public UserGet[] RangeSelect(string token, int skip, int take)
        {
            if (token != "OtQRZmKpl3hpiHXYEys8")
                return null;

            var retval = _userGetRepository.RangeSelect(skip, take);
            return retval.ToArray();
        }
    }
}
