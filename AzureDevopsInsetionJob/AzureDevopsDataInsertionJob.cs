using AzureDevopsDataInsertionJob.Job.Interface;
using AzureDevopsInsetionJob.Domain;
using Microsoft.Extensions.Logging;

namespace AzureDevopsInsetionJob
{
    public class AzureDevopsDataInsertionJob : IJob
    {
        private readonly ILogger<AzureDevopsDataInsertionJob> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public AzureDevopsDataInsertionJob(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AzureDevopsDataInsertionJob>();
            _loggerFactory = loggerFactory;
        }

        public bool StartProcessing()
        {
            _logger.LogInformation("AzureDevopsDataInsertionJob StartProcessingAsync: Started.");
            new AzureDevopsInsetionProcess(_loggerFactory).GenerateRequestFile();
            _logger.LogInformation("AzureDevopsDataInsertionJob StartProcessingAsync: Ended.");
            return true;
        }

    }
}
