using Microsoft.Extensions.Logging;
using System;


namespace AzureDevopsInsetionJob.Logging
{
    public class LogWrapper
    {
        private readonly ILogger<LogWrapper> _logger;
        public LogWrapper(ILogger<LogWrapper> logger)
        {
            _logger = logger;
        }
        public void LevelLogger(string level, string msg)
        {
            if (level == "Trace")
                Trace(msg);
            else if (level == "Info")
                Info(msg);
            else if (level == "Error")
                Error(msg);
            else if (level == "Debug")
                Debug(msg);
            else if (level == "Warn")
                Warn(msg);
        }

        private void Warn(string msg)
        {
            _logger.LogWarning(11, "{msg}", msg);
        }

        private void Debug(string msg)
        {
            _logger.LogDebug(12, "{msg}", msg);
        }

        private void Error(string msg)
        {
            _logger.LogError(13, "{msg}", msg);
        }

        private void Info(string msg)
        {
            _logger.LogInformation(14, "{msg}", msg);
        }

        private void Trace(string msg)
        {
            try
            {
                _logger.LogTrace(15, "{msg}", msg);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
