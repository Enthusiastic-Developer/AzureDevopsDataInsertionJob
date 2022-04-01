using AzureDevopsDataInsertionJob.Job.Interface;
using AzureDevopsInsetionJob.Domain;
using AzureDevopsInsetionJob.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureDevopsInsetionJob
{
    public class AzureDevopsDataInsertionJob : IJob
    {
        #region Class global variables
        private readonly ILogger<AzureDevopsDataInsertionJob> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptions<IMongoDatabaseSettings> _settings = null;
        #endregion
        public AzureDevopsDataInsertionJob(ILoggerFactory loggerFactory, IOptions<IMongoDatabaseSettings> mySettings)
        {
            _logger = loggerFactory.CreateLogger<AzureDevopsDataInsertionJob>();
            _loggerFactory = loggerFactory;
            _settings = mySettings;
        }

        public bool StartProcessing()
        {
            _logger.LogInformation("AzureDevopsDataInsertionJob StartProcessingAsync: Started.");
            new AzureDevopsInsetionProcess(_loggerFactory, _settings).GenerateRequestFile();
            _logger.LogInformation("AzureDevopsDataInsertionJob StartProcessingAsync: Ended.");
            return true;
        }

    }
}
