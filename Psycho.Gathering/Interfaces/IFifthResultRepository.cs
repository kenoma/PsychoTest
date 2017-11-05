using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Interfaces
{
    public interface IFifthResultRepository
    {
        void Insert(int vkId, float[] results);
        void Insert(int[] vkId, float[][] results);
        void CleanAll();
    }
}
