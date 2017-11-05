using Psycho.Common.Repository.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo.SpecialModel
{
    class CareerDb : ILocalAggregateRoot
    {
        public int UserGetId { get; set; }
        public int group_id { get; set; }
        public int country_id { get; set; }
        public int city_id { get; set; }
        public int id { get; set; }
    }
}
