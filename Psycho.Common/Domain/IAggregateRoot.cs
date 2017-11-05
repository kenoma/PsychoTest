using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly:InternalsVisibleTo("Psycho.Service")]
[assembly: InternalsVisibleTo("Psycho.UnitTests")]
namespace Psycho.Common.Domain
{
    public interface IAggregateRoot
    {
        long Id { get; }
    }
}
