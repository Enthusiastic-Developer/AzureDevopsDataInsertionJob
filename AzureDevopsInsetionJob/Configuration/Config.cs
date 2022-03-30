using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace AzureDevopsInsetionJob.Configuration
{
    public class Config : IConfig
    {
        public void ConfigManagerForProgram()
        {
            var config = new ConfigurationBuilder()
                    .Build();
            using var serviceProvider = new ServiceCollection()
                    .AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                        loggingBuilder.AddNLog(config);

                    }).BuildServiceProvider();

        }
    }
}
