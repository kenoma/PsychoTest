using Psycho.Common.Repository.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo.SpecialModel
{
    class WallPostDb : ILocalAggregateRoot
    {
        public int id { get; set; }
        public int UserGetId { get; set; }
        public int from_id { get; set; }
        public int owner_id { get; set; }
        public int date { get; set; }
        public string post_type { get; set; }
        public string post_text { get; set; }
        public bool is_pinned { get; set; }
    }
}
