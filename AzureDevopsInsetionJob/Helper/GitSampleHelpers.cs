using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsInsetionJob.Helper
{
    public class GitSampleHelpers
    {
        public static GitRepository FindAnyRepositoryOnAnyProject(VssConnection connection)
        {
            Guid projectId = ClientSampleHelpers.FindAnyProject(connection).Id;
            return FindAnyRepository(connection, projectId);
        }
        public static GitRepository FindAnyRepository(VssConnection connection, Guid projectId)
        {
            GitRepository repo;
            if (!FindAnyRepository(connection, projectId, out repo))
            {
                throw new Exception("No repositories available. Create a repo in this project and run the sample again.");
            }

            return repo;
        }
        private static bool FindAnyRepository(VssConnection connection, Guid projectId, out GitRepository repo)
        {
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();
            repo = gitClient.GetRepositoriesAsync(projectId).Result.FirstOrDefault();
            return repo != null;
        }
    }
}
