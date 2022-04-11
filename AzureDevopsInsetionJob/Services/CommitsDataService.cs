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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsInsetionJob.Services
{
    public class CommitsDataService
    {
        #region Class global variables
        private readonly ILogger<CommitsDataService> _logger;
        private readonly IOptions<Models.MongoDatabaseSettings> _settings = null;
        private ILoggerFactory _loggerFactory;
        #endregion
        public CommitsDataService(IOptions<Models.MongoDatabaseSettings> mySettings, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CommitsDataService>();
            _loggerFactory = loggerFactory;
            _settings = mySettings;
        }
        protected IMongoDatabaseSettings Settings => _settings.Value;
        public async Task InsertIntoCommitDataAsync()
        {
            try
            {
                var client = new MongoClient(Settings.ConnectionString);
                string ORG = Settings.OrganizationURL;
                string PAT = Settings.PersonalToken;
                _logger.LogInformation("Commits Data insertion is started");
                IMongoDatabase db = client.GetDatabase("AzureDevopsData");
                if (db.CreateCollectionAsync("CommitsData") == null)
                {
                    await db.CreateCollectionAsync("CommitsData");
                }
                IMongoCollection<GitCommitRef> col = db.GetCollection<GitCommitRef>("CommitsData");
                if (ORG != null)
                {
                    Uri orgUrl = new Uri(ORG);
                    string personalToken = PAT;

                    VssConnection connection = new(orgUrl, new VssBasicCredential(string.Empty, personalToken));
                    List<GitCommitRef> commits = new();
                    GitRepository repo = GitSampleHelpers.FindAnyRepositoryOnAnyProject(connection);
                    commits = connection.GetClient<GitHttpClient>().GetCommitsAsync(repo.Id, new GitQueryCommitsCriteria()).Result;
                    foreach (var commit in commits)
                    {
                        GitCommitRef commitRef = new GitCommitRef()
                        {
                            CommitId = commit.CommitId,
                            Author = commit.Author,
                            Committer = commit.Committer,
                            Comment = commit.Comment.ToString(),
                            CommentTruncated = commit.CommentTruncated,
                            Changes = commit.Changes,
                            Parents = commit.Parents,
                            Url = commit.Url,
                            RemoteUrl = commit.RemoteUrl,
                            Links = commit.Links,
                            Statuses = commit.Statuses,
                            WorkItems = commit.WorkItems,
                            Push = commit.Push
                        };
                        await col.InsertOneAsync(commitRef);
                    }
                }
                _logger.LogInformation("Commits Data insertion is started");
            }
            catch (Exception ex)
            {

                _logger.LogError("{0}: {1}", ex.GetType(), ex.Message);
            }
        }
    }
}
