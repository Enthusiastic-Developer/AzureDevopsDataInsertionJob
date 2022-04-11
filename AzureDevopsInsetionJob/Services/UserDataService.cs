using AzureDevopsInsetionJob.Helper;
using AzureDevopsInsetionJob.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureDevopsInsetionJob.Services
{
    public class UserDataService
    {
        #region Class global variables
        private readonly ILogger<UserDataService> _logger;
        private readonly IOptions<Models.MongoDatabaseSettings> _settings = null;
        private ILoggerFactory _loggerFactory;
        #endregion

        public UserDataService(IOptions<Models.MongoDatabaseSettings> mySettings, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UserDataService>();
            _loggerFactory = loggerFactory;
            _settings = mySettings;
        }
        protected IMongoDatabaseSettings Settings => _settings.Value;

        public async Task InsertIntoUserDataAsync()
        {
            try
            {
                var client = new MongoClient(Settings.ConnectionString);
                string ORG = Settings.OrganizationURL;
                string PAT = Settings.PersonalToken;
                _logger.LogInformation("Project Data insertion is started");
                IMongoDatabase db = client.GetDatabase("AzureDevopsData");
                if (db.CreateCollectionAsync("UserData") == null)
                {
                    await db.CreateCollectionAsync("UserData");
                }
                IMongoCollection<GitCommitRef> col = db.GetCollection<GitCommitRef>("UserData");
                if (ORG != null)
                {
                    Uri orgUrl = new Uri(ORG);
                    string personalToken = PAT;

                    VssConnection connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, personalToken));
                    GitHttpClient gitClient = connection.GetClient<GitHttpClient>();
                    var project = ClientSampleHelpers.FindAnyProject(connection);
                    List<GitRepository> repos = gitClient.GetRepositoriesAsync(project.Id).Result;
                    List<GitCommitRef> commits = new List<GitCommitRef>();
                    foreach (var repo in repos)
                    {
                        commits = connection.GetClient<GitHttpClient>().GetCommitsAsync(repo.Id, new GitQueryCommitsCriteria()).Result;
                        foreach (var commit in commits)
                        {
                            GitCommitRef commitRef = new GitCommitRef()
                            {
                                CommitId = commit.CommitId,
                                Author = commit.Author,
                                Comment = commit.Comment,
                                Committer = commit.Committer
                            };
                            await col.InsertOneAsync(commitRef);
                        }
                    }

                }
                _logger.LogInformation("Project Data insertion is Ended");
            }
            catch (Exception ex)
            {

                _logger.LogError("{0}: {1}", ex.GetType(), ex.Message);
            }

        }
    }
}
