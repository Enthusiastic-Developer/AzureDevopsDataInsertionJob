using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;

namespace AzureDevopsInsetionJob.Helper
{
    public static class ClientSampleHelpers
    {
        public static TeamProjectReference FindAnyProject(VssConnection connection)
        {
            if (!FindAnyProject(connection, out TeamProjectReference project))
            {
                throw new Exception("No sample projects available. Create a project in this collection and run the sample again.");
            }
            return project;
        }
        public static bool FindAnyProject(VssConnection connection, out TeamProjectReference project)
        {
            // Check if we already have a default project loaded
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();
            project = projectClient.GetProjects(null, top: 100).Result.FirstOrDefault();
            return project != null;
        }
    }
}
