
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaveCityCenterAPI.Core.Logger
{
    public class CustomLoggerApi : ILog
    {
      
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private IHttpContextAccessor _accessor;
        public CustomLoggerApi(IHttpContextAccessor accessor)
        {
            LoggingConfiguration config = new LoggingConfiguration();
            FileTarget fileTarget = new FileTarget();
            config.AddTarget("logfile", fileTarget);
            fileTarget.FileName = @"C:\Logfile\Logfile.txt";
            NLog.LogManager.Configuration = config;
            LoggingRule rule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule);
            _accessor = accessor;

        }

        public void Information(string message)
        {
            logger.Info(message);
        }

        public void Warning(string message)
        {
            logger.Warn(message);
        }

        public void Debug(string message)
        {
            logger.Debug(message);
        }

        public void Error(string message)
        {
            var EMINumber= _accessor.HttpContext.Request.Headers["EMINumber"].ToString();

            logger.Error(JsonConvert.SerializeObject(_accessor.HttpContext.Request));
        }
    }
}
