using Psycho.Gathering.Models;
using Psycho.Gathering.Models.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Interfaces
{
    public interface IUserGetRepository
    {
        bool SaveUser(UserGet userData, DateTime timestamp);
        
        void SaveUsers(IList<UserGet> users, DateTime timestamp);
        IReadOnlyList<UserGet> GetUser(int id);
        IReadOnlyList<UserGet> GetUsers(int[] id);
        IReadOnlyCollection<UserGetMetaDTO> ListRecords();
        IReadOnlyCollection<UserGet> RangeSelect(int skip, int take);
        IReadOnlyCollection<int> GetUserIds();
        IReadOnlyCollection<int> GetUserVkIds();
        IReadOnlyCollection<Tuple<int, byte[]>> RangeRawSelect(int skip, int take);
        bool SaveRawUsers(IEnumerable<Tuple<int, byte[]>> userData, DateTime timestamp);
        void DeleteUsers(IEnumerable<int> VkIds);
    }
}
