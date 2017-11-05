using Psycho.Gathering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Interfaces
{
    public interface IDenisResultsRepository
    {
        void CleanAll();
        void Insert(IEnumerable<DenisModel> data);
    }
}
