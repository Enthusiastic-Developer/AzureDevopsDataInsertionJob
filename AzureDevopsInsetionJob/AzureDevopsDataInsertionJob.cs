using AzureDevopsDataInsertionJob.Job.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _logger.LogInformation("AzureDevopsDataInsertionJob StartProcessingAsync: Ended.");
            return true;
        }
        
    }
}
