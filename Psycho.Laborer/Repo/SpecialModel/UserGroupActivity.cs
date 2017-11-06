using Psycho.Common.Repository.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo.SpecialModel
{
    class UserGroupActivity : ILocalAggregateRoot
    {
        public int id { get; set; }
        public int UserGetId { get; set; }
        public int GroupId { get; set; }
        public int WallPostId { get; set; }
        public ActivityType ActivityType { get; set; }
        public string PostType { get; set; }
    }
}
