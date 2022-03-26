using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsDataInsertionJob.Job.Interface
{
    public interface IJob
    {
        bool StartProcessing();
    }
}
