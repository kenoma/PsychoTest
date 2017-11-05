using Rebus.Logging;

namespace Psycho.Common.Rebus
{
    class RebusLoggerFactory : IRebusLoggerFactory
    {
        public ILog GetLogger<T>()
        {
            return new RebusLog();
        }
    }
}