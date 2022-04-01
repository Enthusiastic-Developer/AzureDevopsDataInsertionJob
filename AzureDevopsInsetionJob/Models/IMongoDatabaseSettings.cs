using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsInsetionJob.Models
{
    public interface IMongoDatabaseSettings
    {
        public string ConnectionString { get; set; } 
        public string DatabaseName { get; set; } 
        public string CollectionName { get; set; }
        public string OrganizationURL { get; set; }
        public string PersonalToken { get; set; }

    }
}
