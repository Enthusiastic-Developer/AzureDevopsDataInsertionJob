using Microsoft.Extensions.Logging;

namespace AzureDevopsInsetionJob.Logging
{
    public class LogRunner
    {
        private readonly ILogger<LogRunner> _logger;

        public LogRunner(ILogger<LogRunner> logger)
        {
            _logger = logger;
        }
        public void DoAction(string name)
        {
            _logger.LogDebug(20, "Logger is working{name}", name);
        }
    }
}
