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
    public class RepoDataService
    {
        #region Class global variables
        private readonly ILogger<RepoDataService> _logger;
        private readonly IOptions<Models.MongoDatabaseSettings> _settings = null;
        private ILoggerFactory _loggerFactory;
        #endregion
        public RepoDataService(IOptions<Models.MongoDatabaseSettings> mySettings, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RepoDataService>();
            _loggerFactory = loggerFactory;
            _settings = mySettings;
        }
        protected IMongoDatabaseSettings Settings => _settings.Value;
        public async Task InsertIntoRepoDataAsync()
        {
            try
            {
                var client = new MongoClient(Settings.ConnectionString);
                string ORG = Settings.OrganizationURL;
                string PAT = Settings.PersonalToken;
                _logger.LogInformation("Repo Data insertion is started");
                IMongoDatabase db = client.GetDatabase("AzureDevopsData");
                if (db.CreateCollectionAsync("ReposData") == null)
                {
                    await db.CreateCollectionAsync("ReposData");
                }
                IMongoCollection<GitRepository> col = db.GetCollection<GitRepository>("ReposData");
                if (ORG != null)
                {
                    Uri orgUrl = new Uri(ORG);
                    string personalToken = PAT;

                    VssConnection connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, personalToken));
                    GitHttpClient gitClient = connection.GetClient<GitHttpClient>();
                    Guid projectId = ClientSampleHelpers.FindAnyProject(connection).Id;
                    List<GitRepository> repos = gitClient.GetRepositoriesAsync(projectId).Result;
                    foreach (GitRepository repo in repos)
                    {
                        await col.InsertOneAsync(repo);
                    }
                }
                _logger.LogInformation("Repo Data insertion is Ended");
                Console.WriteLine("RepoDataService is Completed");
            }
            catch (Exception ex)
            {

                _logger.LogError("{0}: {1}", ex.GetType(), ex.Message);
            }

        }
    }
}
