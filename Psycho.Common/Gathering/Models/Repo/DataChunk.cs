using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Gathering.Models.Repo
{
    public class DataChunk
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int VkontakteUserId { get; set; }
        public byte[] CompressedUserGet { get; set; }
    }
}
