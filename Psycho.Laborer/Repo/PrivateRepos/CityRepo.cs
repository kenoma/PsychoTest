using Psycho.Gathering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo
{
    class CityRepo : Repository<City>
    {
        public CityRepo() 
            : base(nameof(City))
        {
        }
    }
}
