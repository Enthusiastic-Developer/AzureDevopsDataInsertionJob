using AzureDevopsDataInsertionJob.Job.Interface;
using AzureDevopsInsetionJob.Configuration;
using AzureDevopsInsetionJob.Models;
using AzureDevopsInsetionJob.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureDevopsInsetionJob
{
    class Program
    {
        #region Class global variables
        private static ILoggerFactory _loggerFactory;
        private static readonly IOptions<IMongoDatabaseSettings> _settings = null;
        #endregion
        static async Task Main(string[] args)
        {
            try
            {
                IConfig config = new Config();
                await config.ConfigManagerForProgram();
                ConfigureLogger();
                IJob job = new AzureDevopsDataInsertionJob(_loggerFactory, _settings);
                job.StartProcessing();
            }
            catch (Exception ex)
            {

                Console.WriteLine("{0}: {1}", ex.GetType(), ex.Message);
            }
        }
        /// <summary>
        /// Methos for Nlog configuration
        /// </summary>
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
