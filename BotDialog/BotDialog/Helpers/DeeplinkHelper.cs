using TeamsHub.SiteRequest.Helper;
using System.Web;

namespace TeamsHub.SiteRequest.Helpers
{
    public static class DeeplinkHelper
    {
        public static string GetLeaveBoardDeeplink(string emailId)
        {
            return $"https://teams.microsoft.com/l/entity/{ApplicationSettings.AppId}/com.contoso.SiteRequest.leaveboard?webUrl={HttpUtility.UrlEncode(ApplicationSettings.BaseUrl + "?EmailId=" + emailId)}&label=Leave%20Board";
        }

        public static string PublicHolidaysDeeplink { get; set; } =
            $"https://teams.microsoft.com/l/entity/{ApplicationSettings.AppId}/com.contoso.SiteRequest.holidays?webUrl={HttpUtility.UrlEncode(ApplicationSettings.BaseUrl + "/first")}&label=Public%20Holidays";

        public static string HelpDeeplink { get; set; } =
            $"https://teams.microsoft.com/l/entity/{ApplicationSettings.AppId}/com.contoso.SiteRequest.help?webUrl={HttpUtility.UrlEncode(ApplicationSettings.BaseUrl + "/second")}&label=Help";
    }
}