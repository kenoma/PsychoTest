using Psycho.Gathering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo
{
    class UniversityRepo : Repository<University>
    {
        public UniversityRepo() 
            : base(nameof(University))
        {
        }
    }
}
