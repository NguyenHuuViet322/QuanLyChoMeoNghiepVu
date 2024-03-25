using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Utility.LogUtils
{
    public class LoggerManager: ILoggerManager
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        public void LogDebug(string message)
        {
            logger.Debug(message);
        }
        public void LogError(string message)
        {
            logger.Error(message);
        }
        
        public void LogError(Exception ex)
        {
            logger.Error(ex);
        }

        public void LogInfo(string message)
        {
            logger.Info(message);
        }
        public void LogWarn(string message)
        {
            logger.Warn(message);
        }

    }
}
