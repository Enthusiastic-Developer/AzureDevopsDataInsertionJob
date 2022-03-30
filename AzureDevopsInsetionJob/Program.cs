using AzureDevopsDataInsertionJob.Job.Interface;
using AzureDevopsInsetionJob.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;

namespace AzureDevopsInsetionJob
{
    class Program
    {
        private static ILoggerFactory _loggerFactory;
        static void Main(string[] args)
        {
            IConfig config = new Config();
            config.ConfigManagerForProgram();
            ConfigureLogger();
            IJob job = new AzureDevopsDataInsertionJob(_loggerFactory);
            job.StartProcessing();
        }
        private static void ConfigureLogger()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, NLogLoggerFactory>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            GlobalDiagnosticsContext.Set("basedir", "AzureDevopsInsetionJob");
        }
    }
}
