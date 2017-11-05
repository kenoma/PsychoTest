using Psycho.Common.Service.Messages;
using Psycho.Laborer.Infrastructure;
using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Laborer.Handlers
{
    class HandleGetGroupLikesRepostsComments : IHandleMessages<MessageGetGroupLikesRepostsComments>
    {
        private UnitsProvider _unitsProvider;

        public HandleGetGroupLikesRepostsComments(UnitsProvider unitsProvider)
        {
            _unitsProvider = unitsProvider;
        }

        async public Task Handle(MessageGetGroupLikesRepostsComments message)
        {
            
        }
    }
}
