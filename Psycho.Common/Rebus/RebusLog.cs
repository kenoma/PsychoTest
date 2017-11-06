using Psycho.Common.Service;
using Rebus.Logging;
using Serilog;
using Serilog.Core;
using System;

namespace Psycho.Common.Rebus
{
    class RebusLog : ILog
    {
        private ILogger _log;

        public RebusLog()
        {
            _log = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.LiterateConsole()
                .CreateLogger();
        }

        public void Debug(string message, params object[] objs)
        {
            _log.Debug(message, objs);
        }

        public void Error(Exception exception, string message, params object[] objs)
        {
            Log.Error(exception, message, CallerInfo.Create(), objs);
        }

        public void Error(string message, params object[] objs)
        {
            Log.Error(message, CallerInfo.Create(), objs);
        }

        public void Info(string message, params object[] objs)
        {
            _log.Information(message, objs);
        }

        public void Warn(string message, params object[] objs)
        {
            _log.Warning(message, objs);
        }

        public void Warn(Exception exception, string message, params object[] objs)
        {
            _log.Error(exception, message, objs);
        }
    }
}