using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Service
{
    public static class LogHelper
    {
        static public ILogger InitLog()
        {
            return new LoggerConfiguration()
              .MinimumLevel.Verbose()
              .WriteTo.LiterateConsole()
              .WriteTo.Seq(CommonConfig.Default.SeqServer, compact: true)
              .CreateLogger();
        }
    }
}
