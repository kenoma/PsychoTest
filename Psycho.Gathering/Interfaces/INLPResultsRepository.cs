using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Interfaces
{
    public interface INLPResultsRepository
    {
        void CleanAll();
        void Insert(int[] vkId, float[][] results);
    }
}
