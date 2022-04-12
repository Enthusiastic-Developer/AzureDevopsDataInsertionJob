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
using System.Threading.Tasks;

namespace AzureDevopsInsetionJob.Services
{
    public class ChangesDataService
    {
        #region Class global variables
        private readonly ILogger<ChangesDataService> _logger;
        private readonly IOptions<Models.MongoDatabaseSettings> _settings = null;
        private ILoggerFactory _loggerFactory;
        #endregion
        public ChangesDataService(IOptions<Models.MongoDatabaseSettings> mySettings, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ChangesDataService>();
            _loggerFactory = loggerFactory;
            _settings = mySettings;
        }
        protected IMongoDatabaseSettings Settings => _settings.Value;
        public async Task InsertIntoChangesDataAsync()
        {
            try
            {
                var client = new MongoClient(Settings.ConnectionString);
                string ORG = Settings.OrganizationURL;
                string PAT = Settings.PersonalToken;
                _logger.LogInformation("Changes Data insertion is started");
                IMongoDatabase db = client.GetDatabase("AzureDevopsData");
                if (db.CreateCollectionAsync("ChangesData") == null)
                {
                    await db.CreateCollectionAsync("ChangesData");
                }
                IMongoCollection<GitChange> col = db.GetCollection<GitChange>("ChangesData");
                if (ORG != null)
                {
                    Uri orgUrl = new Uri(ORG);
                    string personalToken = PAT;

                    VssConnection connection = new(orgUrl, new VssBasicCredential(string.Empty, personalToken));
                    GitCommitChanges changes = new();
                    List<GitCommitRef> commits = new();
                    int Countvalue = 1000;
                    TeamProjectReference project = ClientSampleHelpers.FindAnyProject(connection);
                    GitRepository repo = GitSampleHelpers.FindAnyRepository(connection, project.Id);
                    commits = connection.GetClient<GitHttpClient>().GetCommitsAsync(repo.Id, new GitQueryCommitsCriteria()).Result;
                    foreach (var commit in commits)
                    {
                        changes = connection.GetClient<GitHttpClient>().GetChangesAsync(commit.CommitId, repo.Id, skip: Countvalue).Result;
                        foreach (var Change in changes.Changes)
                        {
                            await col.InsertOneAsync(Change);
                        }
                    }

                }
                _logger.LogInformation("Changes Data insertion is Ended");
                Console.WriteLine("ChangesDataService is Completed");
            }
            catch (Exception ex)
            {

                _logger.LogError("{0}: {1}", ex.GetType(), ex.Message);
            }
        }
    }
}
