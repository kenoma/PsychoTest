using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Models.Repo
{
    public class WallPostDataChunk
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public byte[] CompressedData { get; set; }
    }
}
