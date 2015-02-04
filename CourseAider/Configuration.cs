using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CourseAider
{
    public static class Configuration
    {
        private static string GetConfigurationByKey(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string IrcServerUri
        {
            get
            {
                return GetConfigurationByKey("CourseAider.IRC.Server");
            }
        }
    }
}