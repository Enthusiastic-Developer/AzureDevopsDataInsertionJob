using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace AzureDevopsInsetionJob.Domain
{
    public class AzureDevopsInsetionProcess
    {
        #region global variable declaration
        private readonly ILogger<AzureDevopsInsetionProcess> _logger;
        private ILoggerFactory _loggerFactory;
        #endregion

        public AzureDevopsInsetionProcess(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AzureDevopsInsetionProcess>();
            _loggerFactory = loggerFactory;
        }

        public void GenerateRequestFile()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            string ORG = configuration.GetSection("ProjectInformation:OrganizationURL").Value;
            string PAT = configuration.GetSection("ProjectInformation:PersonalToken").Value;
            if (ORG != null)
            {
                Uri orgUrl = new Uri(ORG);
                string personalToken = PAT;

                VssConnection connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, personalToken));

                FetchAllProjects(connection);
            }
        }
        private void FetchAllProjects(VssConnection connection)
        {
            try
            {
                ProjectHttpClient httpClient = connection.GetClient<ProjectHttpClient>();
                IEnumerable<TeamProjectReference> projects = httpClient.GetProjects().Result;
                foreach (TeamProjectReference item in projects)
                {
                    Console.WriteLine($"ProjectName : {item.Id}");
                    Console.WriteLine($"ProjectName : {item.Name}");
                    Console.WriteLine($"ProjectName : {item.Description}");
                    Console.WriteLine($"ProjectName : {item.Visibility}");
                    throw new Exception("Testing LogError");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}: {1}", ex.GetType(), ex.Message);
            }

        }

    }
}
