using Psycho.Common.Repository.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo.SpecialModel
{
    class FriendsFollowersSubscriptions : ILocalAggregateRoot
    {
        public int UserGetId { get; set; }
        public int SubjectId { get; set; }
        public FFSType RelationsType { get; set; }
        public int id { get; set; }
    }
}
