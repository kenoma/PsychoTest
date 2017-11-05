using Psycho.Gathering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Interfaces
{
    public interface IPeterResultsRepository
    {
        void CleanAll();
        void Insert(IEnumerable<PeterModel> data);
    }
}
