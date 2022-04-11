namespace AzureDevopsInsetionJob.Models
{
    public class MongoDatabaseSettings : IMongoDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string CollectionName { get; set; } = null!;
        public string OrganizationURL { get; set; } = null!;
        public string PersonalToken { get; set; } = null!;
    }
}
