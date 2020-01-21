using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BotDialog.Helpers
{
    public static class ApplicationSettings
    {
        public static string BaseUrl { get; set; }

        public static string ConnectionName { get; set; }

        public static string AppId { get; set; }

        static ApplicationSettings()
        {
            ConnectionName = ConfigurationManager.AppSettings["ConnectionName"];
            BaseUrl = ConfigurationManager.AppSettings["BaseUri"];

            AppId = ConfigurationManager.AppSettings["MicrosoftAppId"];
        }
    }
}