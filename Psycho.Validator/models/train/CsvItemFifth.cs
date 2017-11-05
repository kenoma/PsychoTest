using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Validator.models.train
{
    [DebuggerDisplay("{vkid}\t[{a1}, {e1}, {c1}, {n1}, {o1}]")]
    class CsvItemFifth
    {
        public int vkid { get; set; }
        public int e1 { get; set; }
        public int a1 { get; set; }
        public int c1 { get; set; }
        public int n1 { get; set; }
        public int o1 { get; set; }
        
        public double[] vector => new double[]
        {
            e1,
            a1,
            c1,
            n1,
            o1
        };

        public double[] GetNormDeviation(double[] norm, double[] dev)
        {
            var retval = vector;
            for (int i = 0; i < retval.Length; i++)
            {
                retval[i] = (retval[i] - norm[i]) / dev[i];
            }
            return retval;
        }
    }
}
