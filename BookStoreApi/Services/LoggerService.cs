using BookStoreApi.Contracts;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Services
{
    public class LoggerService : ILoggerService
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

        public void LogInfo(string message)
        {
            logger.Info(message);
        }

        public void LogWarn(string message)
        {
            logger.Warn(message);
        }

        public void LogMessage(LoggerLevel level, string message)
        {
            switch (level)
            {
                case LoggerLevel.Debug:
                    LogDebug(message);
                    break;

                case LoggerLevel.Error:
                    LogError(message);
                    break;

                case LoggerLevel.Warn:
                    LogWarn(message);
                    break;

                default:
                    LogInfo(message);
                    break;

            }
        }

        public void LogServerError(string msg)
        {
            LogError($"Server Error: {msg}");
        }

        public void LogServerError(Exception e)
        {
            LogServerError($"{ e.Message} - {e.InnerException}");
        }
    }
}
