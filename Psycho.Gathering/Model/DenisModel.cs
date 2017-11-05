using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Model
{
    public class DenisModel
    {
        public int user_id { get; set; }
        public string gender { get; set; }
        public string age { get; set; }
        public double opp_count { get; set; }
        public double pat_count_if_opp { get; set; }
        public double stage_1 { get; set; }
        public double stage_2 { get; set; }
        public double stage_3 { get; set; }
        public double stage_4 { get; set; }
        public double stage_5 { get; set; }
        public double stage_6 { get; set; }
    }
}
