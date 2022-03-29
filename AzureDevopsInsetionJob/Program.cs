using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog;
using System;
using AzureDevopsDataInsertionJob.Job.Interface;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using AzureDevopsInsetionJob.Logging;
using AzureDevopsInsetionJob.Configuration;

namespace AzureDevopsInsetionJob
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfig config = new Config();
            config.ConfigManagerForProgram();
        }
    }
}
