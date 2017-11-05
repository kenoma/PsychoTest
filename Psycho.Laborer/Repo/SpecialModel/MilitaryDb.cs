using Psycho.Common.Repository.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Repo.SpecialModel
{
    class MilitaryDb
    {
        public int Id { get; set; }
        
        public string Unit { get; set; }
        
        public int Unit_Id { get; set; }
        
        public int Country_Id { get; set; }
        public int UserGetId { get; set; }
    }
}
