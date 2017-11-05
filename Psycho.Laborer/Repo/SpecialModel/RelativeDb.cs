using Psycho.Common.Repository.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo.SpecialModel
{
    class RelativeDb
    {
        public int id { get; set; }
        public int RelativeId { get; set; }
        public int UserGetId { get; set; }
        public string Type { get; set; }
    }
}
