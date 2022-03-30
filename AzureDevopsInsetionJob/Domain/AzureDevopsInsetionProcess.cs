using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace AzureDevopsInsetionJob.Domain
{
    public class AzureDevopsInsetionProcess
    {
        private ILoggerFactory _loggerFactory;

        public AzureDevopsInsetionProcess(ILoggerFactory loggerFactory)
        {
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
            Console.WriteLine($"{ORG}");
            Console.WriteLine($"{PAT}");
            //if (ORG != null)
            //{
            //    Uri orgUrl = new Uri(ORG);
            //    String personalToken = PAT;

            //    VssConnection connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, personalToken));

            //    FetchAllProjects(connection);
            //}
        }
        private static void FetchAllProjects(VssConnection connection)
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
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("{0}: {1}", ex.GetType(), ex.Message);
            }

        }

    }
}
