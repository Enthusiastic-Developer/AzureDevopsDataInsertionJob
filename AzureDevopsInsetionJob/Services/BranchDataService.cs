using AzureDevopsInsetionJob.Helper;
using AzureDevopsInsetionJob.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureDevopsInsetionJob.Services
{
    public class BranchDataService
    {
        #region Class global variables
        private readonly ILogger<BranchDataService> _logger;
        private readonly IOptions<Models.MongoDatabaseSettings> _settings = null;
        private ILoggerFactory _loggerFactory;
        #endregion
        public BranchDataService(IOptions<Models.MongoDatabaseSettings> mySettings, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BranchDataService>();
            _loggerFactory = loggerFactory;
            _settings = mySettings;
        }
        protected IMongoDatabaseSettings Settings => _settings.Value;
        public async Task InsertIntoBranchDataAsync()
        {
            try
            {
                var client = new MongoClient(Settings.ConnectionString);
                string ORG = Settings.OrganizationURL;
                string PAT = Settings.PersonalToken;
                _logger.LogInformation("Branch Data insertion is started");
                IMongoDatabase db = client.GetDatabase("AzureDevopsData");
                if (db.CreateCollectionAsync("BranchData") == null)
                {
                    await db.CreateCollectionAsync("BranchData");
                }
                IMongoCollection<GitBranchStats> col = db.GetCollection<GitBranchStats>("BranchData");
                if (ORG != null)
                {
                    Uri orgUrl = new Uri(ORG);
                    string personalToken = PAT;

                    VssConnection connection = new(orgUrl, new VssBasicCredential(string.Empty, personalToken));
                    GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

                    TeamProjectReference project = ClientSampleHelpers.FindAnyProject(connection);
                    GitRepository repo = GitSampleHelpers.FindAnyRepository(connection, project.Id);

                    // find a handful of branches to compare
                    List<GitRef> branches = gitClient.GetRefsAsync(repo.Id, filter: "heads/").Result;
                    IEnumerable<string> branchNames = from branch in branches
                                                      where branch.Name.StartsWith("refs/heads/")
                                                      select branch.Name["refs/heads/".Length..];

                    if (branches.Count < 1)
                    {
                        _logger.LogInformation($"Repo {repo.Name} doesn't have any branches in it.");
                    }

                    if (string.IsNullOrEmpty(repo.DefaultBranch))
                    {
                        _logger.LogInformation($"Repo {repo.Name} doesn't have a default branch");
                    }
                    string defaultBranchName = repo.DefaultBranch["refs/heads/".Length..];

                    // list up to 10 branches we're interested in comparing
                    GitQueryBranchStatsCriteria criteria = new GitQueryBranchStatsCriteria()
                    {
                        baseVersionDescriptor = new GitVersionDescriptor
                        {
                            VersionType = GitVersionType.Branch,
                            Version = defaultBranchName,
                        },
                        targetVersionDescriptors = branchNames
                            .Take(10)
                            .Select(branchName => new GitVersionDescriptor()
                            {
                                Version = branchName,
                                VersionType = GitVersionType.Branch,
                            })
                            .ToArray()
                    };

                    List<GitBranchStats> stats = gitClient.GetBranchStatsBatchAsync(criteria, repo.Id).Result;

                    foreach (GitBranchStats stat in stats)
                    {
                        await col.InsertOneAsync(stat);
                    }
                }
                _logger.LogInformation("Branch Data insertion is Ended");
                Console.WriteLine("BranchDataService is Completed");
            }
            catch (Exception ex)
            {

                _logger.LogError("{0}: {1}", ex.GetType(), ex.Message);
            }
        }
    }
}
