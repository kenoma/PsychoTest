using Psycho.Gathering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo
{
    class SchoolRepo : Repository<School>
    {
        public SchoolRepo() 
            : base(nameof(School))
        {
        }
    }
}
