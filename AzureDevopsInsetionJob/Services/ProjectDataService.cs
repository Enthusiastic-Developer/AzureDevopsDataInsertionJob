using AzureDevopsInsetionJob.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureDevopsInsetionJob.Services
{
    public class ProjectDataService
    {
        #region Class global variables
        private readonly ILogger<ProjectDataService> _logger;
        private readonly IOptions<Models.MongoDatabaseSettings> _settings = null;
        private ILoggerFactory _loggerFactory;
        #endregion
        public ProjectDataService(IOptions<Models.MongoDatabaseSettings> mySettings, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ProjectDataService>();
            _loggerFactory = loggerFactory;
            _settings = mySettings;
        }
        protected IMongoDatabaseSettings Settings => _settings.Value;
        public async Task InsertIntoProjectsDataAsync()
        {
            try
            {
                var client = new MongoClient(Settings.ConnectionString);
                string ORG = Settings.OrganizationURL;
                string PAT = Settings.PersonalToken;
                _logger.LogInformation("Project Data insertion is started");
                IMongoDatabase db = client.GetDatabase("AzureDevopsData");

                if (db.CreateCollectionAsync("ProjectData") == null)
                {
                    await db.CreateCollectionAsync("ProjectData");
                }
                IMongoCollection<TeamProjectReference> col = db.GetCollection<TeamProjectReference>("ProjectData");
                var tasks = col.Find(new BsonDocument()).ToList();
                if (ORG != null)
                {
                    Uri orgUrl = new Uri(ORG);
                    string personalToken = PAT;

                    VssConnection connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, personalToken));
                    ProjectHttpClient httpClient = connection.GetClient<ProjectHttpClient>();
                    IEnumerable<TeamProjectReference> projects = httpClient.GetProjects().Result;
                    foreach (TeamProjectReference projectReference in projects)
                    {
                        await col.InsertOneAsync(projectReference);
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
