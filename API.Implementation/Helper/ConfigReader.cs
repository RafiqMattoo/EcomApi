using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace API.Helper
{
    public class ConfigReader
    {
      

        public static string GetVariable(string nodeName)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                          .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                          .AddJsonFile("appsettings.json")
                          .Build();
            var decode = configuration.GetSection("AppSettings").GetSection("Decode").Value;

            return configuration.GetSection("AppSettings").GetSection(nodeName).Value;
            
           

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectionName"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static string GetVariable(string selectionName, string nodeName)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                          .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                          .AddJsonFile("appsettings.json")
                          .Build();
            var decode = configuration.GetSection("AppSettings").GetSection("Decode").Value;

           return configuration.GetSection(selectionName).GetSection(nodeName).Value;
            
        }

 
   

    }
}
