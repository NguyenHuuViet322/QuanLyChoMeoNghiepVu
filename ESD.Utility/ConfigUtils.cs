using Microsoft.Extensions.Configuration;
using System;

namespace ESD.Utility
{
    public static class ConfigUtils
    {
        public static string GetConnectionString(string key)
        {
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { @"bin\" }, StringSplitOptions.None)[0];
         IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
                .Build();
            return configuration.GetConnectionString(key);
        }

        public static string GetKeyValue(string section,string key)
        {
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(projectPath)
                   .AddJsonFile("appsettings.json")
                   .Build();
            return configuration.GetSection(section)[key];
        }
        /// <summary>
        /// Lấy applycation cấp 1
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetKeyValue(string key)
        {
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(projectPath)
                   .AddJsonFile("appsettings.json")
                   .Build();
            return configuration[key];
        }

        /// <summary>
        /// Lấy setting theo key theo kiểu dữ liệu
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetAppSetting<T>(string key)
        {
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(projectPath)
                   .AddJsonFile("appsettings.json")
                   .Build();
            return configuration.GetSection(key).Get<T>();
        }
    }

}
