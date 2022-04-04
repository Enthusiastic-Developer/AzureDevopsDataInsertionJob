using AzureDevopsInsetionJob.Models;
using AzureDevopsInsetionJob.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureDevopsInsetionJob.Configuration
{
    public class Config : IConfig
    {
        public async Task ConfigManagerForProgram()
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(env))
            {
                env = "Development";
            }
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                                .AddEnvironmentVariables();
            IConfigurationRoot configuration = builder.Build();
            var services = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    loggingBuilder.AddNLog(configuration);

                });
            services.AddTransient<ProjectDataService>();
            services.AddTransient<UserDataService>();
            services.Configure<MongoDatabaseSettings>(configuration.GetSection("MongoConnection"));
            services.Configure<MongoDatabaseSettings>(configuration.GetSection("ProjectInformation"));
            var provider = services.BuildServiceProvider();

            var PDS = provider.GetService<ProjectDataService>();
            var UDS = provider.GetService<UserDataService>();
            await PDS.InsertIntoProjectsDataAsync();
            await UDS.InsertIntoUserDataAsync();
        }
    }
}
