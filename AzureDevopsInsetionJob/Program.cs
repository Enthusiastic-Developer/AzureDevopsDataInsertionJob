using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog;
using System;
using AzureDevopsDataInsertionJob.Job.Interface;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using AzureDevopsInsetionJob.Configuration;

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
