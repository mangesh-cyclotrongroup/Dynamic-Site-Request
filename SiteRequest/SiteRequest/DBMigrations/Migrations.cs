using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteRequest.DBMigrations
{
    /// <summary>
    /// Manages all updates/migration to DB for further deployments
    /// </summary>
    public static class Migrations
    {
        public static Dictionary<int, string> migrationData = new Dictionary<int, string>() { 
            { 1, "CREATE TABLE UserLicense (Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT UNIQUE NOT NULL, Status INTEGER);" },
            { 2, "ALTER TABLE UserNav ADD Groups TEXT;" },
            { 3, "ALTER TABLE AdminSettings ADD ProvisionType TEXT; ALTER TABLE SiteProvision ADD SiteTemplateType TEXT; ALTER TABLE SiteProvision ADD ApprovedBy TEXT;"  },
            { 4, "ALTER TABLE SiteProvision ADD RequestedDate TEXT; ALTER TABLE SiteProvision ADD ApprovedDate TEXT; ALTER TABLE SiteProvision ADD Comments TEXT;"  },
            { 5, "ALTER TABLE SiteProvision ADD Members TEXT; ALTER TABLE SiteProvision ADD SiteDesignTemplate TEXT;"  }
        };
    }
}