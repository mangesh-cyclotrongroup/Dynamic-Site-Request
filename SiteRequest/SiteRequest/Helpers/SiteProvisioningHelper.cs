using Newtonsoft.Json;
using SiteRequest.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotDialog.Helpers
{
    public static class SiteProvisioningHelper
    {
        public static string getSiteProvisionConfig()
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var dt = sqLiteDatabase.GetDataTable("SELECT Key,IsSiteProvisionEnabled,ProvisionType FROM AdminSettings");
                    JSONString = JsonConvert.SerializeObject(dt);
                    sqLiteDatabase.CloseConnection();
                }
                return JSONString;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Datacontroller : " + ex.Message);
                return null;
            }
        }
    }
}