using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Domain.UserData.Guts
{
    public class ActivityData
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public ActivityCode Code { get; set; }

        public long RelatedScopeId { get; set; }
    }
}
