using Psycho.Gathering.Models;
using Psycho.Gathering.Models.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Interfaces
{
    public interface IAntiBotRepository
    {
        void SaveUsers(IEnumerable<Tuple<int, bool>> isBotData);
        IReadOnlyCollection<int> GetUserVkIds();
    }
}
