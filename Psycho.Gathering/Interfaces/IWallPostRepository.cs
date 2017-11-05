using Psycho.Gathering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Interfaces
{
    public interface IWallPostRepository
    {
        bool SaveWallpost(WallResponse wallData);
        IReadOnlyCollection<WallResponse> RangeSelect(int skip, int take);
        IReadOnlyCollection<byte[]> RangeRawSelect(int skip, int take);
    }
}
