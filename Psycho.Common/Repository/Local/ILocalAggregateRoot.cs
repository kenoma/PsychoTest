using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Repository.Local
{
    public interface ILocalAggregateRoot
    {
        int id { get; set; }
    }
}
