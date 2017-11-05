using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Validator.models.train
{
    [DebuggerDisplay("{vkid}\t[{y1_universe}, {y2_samost}, {y3_polnota}, {y4_gedonizm}, {y5_dostij}, {y6_power}, {y7_security}, {y8_social}, {y9_tradition}, {y10_goodwill}]")]
    class CsvItem
    {
        public int vkid { get; set; }
        public int y1_universe { get; set; }
        public int y2_samost { get; set; }
        public int y3_polnota { get; set; }
        public int y4_gedonizm { get; set; }
        public int y5_dostij { get; set; }
        public int y6_power { get; set; }
        public int y7_security { get; set; }
        public int y8_social { get; set; }
        public int y9_tradition { get; set; }
        public int y10_goodwill { get; set; }

        public double[] vector => new double[]
        {
            y1_universe ,
            y2_samost   ,
            y3_polnota  ,
            y4_gedonizm ,
            y5_dostij   ,
            y6_power    ,
            y7_security ,
            y8_social   ,
            y9_tradition,
            y10_goodwill,
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
