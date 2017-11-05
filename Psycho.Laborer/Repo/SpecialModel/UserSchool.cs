using Psycho.Common.Repository.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo.SpecialModel
{
    class UserSchool: ILocalAggregateRoot
    {
        public int id { get; set; }
        public int UserGetId { get; set; }
        public int SchoolId { get; set; }
    }
}
