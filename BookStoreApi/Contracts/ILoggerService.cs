using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Contracts
{
    public enum LoggerLevel
    {
        Info,
        Warn,
        Error,
        Debug
    }
    public interface ILoggerService
    {
        void LogInfo(string message);
        void LogWarn(string message);
        void LogError(string message);
        void LogDebug(string message);

        void LogMessage(LoggerLevel level, string message);
        void LogServerError(string msg);
        void LogServerError(Exception e);
    }
}
