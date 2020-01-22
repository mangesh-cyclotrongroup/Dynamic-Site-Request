using SiteRequest.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using SiteRequest.Helpers;

namespace SiteRequest.DBMigrations
{
    public class DbMigrationHelper
    {
        private static string _migrationTableName = "MigrationHistory";

        public static void ExecuteMigrations()
        {
            try
            {
                if (!CheckMigrationHistoryTable())
                {
                    CreateMigrationHistoryTable();
                }
                
                var pendingMigrations = GetPendingMigrationsToApply();

                if (pendingMigrations.Any())
                {
                    ApplyMigrations(pendingMigrations);
                }
            }
            catch (Exception)
            {

            }
        }
        private static List<Tuple<string, string, string, string, int, string>> _featureList = new List<Tuple<string, string, string, string, int, string>>
        {
            Tuple.Create("TeamsHub Admin","Site Provision","Enable or Disable Site Provision features in TeamsHub","True", 1, "Released"),
            Tuple.Create("TeamsHub Admin","NewsLetter","NewsLetter","True", 2, "Released"),
            Tuple.Create("TeamsHub Admin","Teams Voice Routing","Configure Voice Routing","True", 3, "Released"),          
            Tuple.Create("TeamsHub Admin","Teams Template genarator (Coming Soon)","Teams Template genarator","False", 6, "Development"),
            Tuple.Create("TeamsHub Admin","Enhanced Teams Provision Engine (Coming soon)","Enhanced Teams Provision Engine","False",7, "Development"),
                      
            Tuple.Create("TeamsHub Bots","Site Request","Site Request Bot","True",9, "Released"),
            Tuple.Create("TeamsHub Bots","Company Communicator (Coming Soon)","Bot for Company Communicator","False",10, "Development"),
            Tuple.Create("TeamsHub Bots","Birthdays and Anniversaries (Beta)","Birthdays and Anniversaries","False",11, "Beta"),
            Tuple.Create("TeamsHub Bots","Mark yourself safe (Coming soon)","Mark yourself safe","False",12, "Development"),
            Tuple.Create("TeamsHub Bots","WhoIs/skills (Coming soon)","AD integration ","False",13, "Development"),
            Tuple.Create("TeamsHub Bots","Incident reporting (Coming soon)","Incident reporting","False",14, "Development"),
            Tuple.Create("TeamsHub Bots","Time/expense reporting (Coming soon)","Time/expense reporting ","False",15, "Development"),
            Tuple.Create("TeamsHub Bots","Request/project Intake (Coming soon)","Request/project Intake","False",16, "Development")
        };
        public static void DropAndCreateTeamsHubFeaturesTable()
        {
            using (var sqLiteDatabase = new SqLiteDatabase())
            {
                sqLiteDatabase.OpenConnection();
                string deleteQuery = $"DROP TABLE TeamsHubFeatures";
                string createquery = $"CREATE TABLE TeamsHubFeatures (Feature TEXT, SubFeature TEXT, Description TEXT,Enabled TEXT,DisplayOrder INTEGER,Status TEXT)";
                var insertqueryArray = _featureList.Select(t => $"INSERT INTO TeamsHubFeatures (Feature,SubFeature,Description,Enabled,DisplayOrder) VALUES ('{t.Item1}','{t.Item2}','{t.Item3}','{t.Item4}','{t.Item5}' )").ToList();
                var queryList = new List<string>();
                queryList.Add(deleteQuery);
                queryList.Add(createquery);
                queryList.AddRange(insertqueryArray);
                var query = string.Join(";", queryList);
                sqLiteDatabase.ExecuteNonQuery(query);
                sqLiteDatabase.CloseConnection();
            }
        }

        private static bool CheckMigrationHistoryTable()
        {
            bool tableExists = false;
            using (var sqLiteDatabase = new SqLiteDatabase())
            {
                sqLiteDatabase.OpenConnection();
                string query = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{_migrationTableName}';";
                string value = sqLiteDatabase.ExecuteScalar(query);
                tableExists = !string.IsNullOrWhiteSpace(value);
                sqLiteDatabase.CloseConnection();
            }
            return tableExists;
        }

        private static bool CreateMigrationHistoryTable()
        {
            try
            {
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    string createquery = $"CREATE TABLE {_migrationTableName} (Version TEXT)";
                    sqLiteDatabase.ExecuteNonQuery(createquery);
                    sqLiteDatabase.CloseConnection();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        private static IEnumerable<KeyValuePair<int,string>> GetPendingMigrationsToApply()
        {
            int maxVersion = -1;
            try
            {
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    string createquery = $"SELECT MAX(Version) FROM {_migrationTableName}; ";
                    string maxVersionInDb = sqLiteDatabase.ExecuteScalar(createquery);

                    maxVersion = string.IsNullOrWhiteSpace(maxVersionInDb) ? -1 : int.Parse(maxVersionInDb);

                    sqLiteDatabase.CloseConnection();
                }
                return Migrations.migrationData.Where(x => x.Key > maxVersion).OrderBy(x=>x.Key);
                    
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static bool ApplyMigrations(IEnumerable<KeyValuePair<int,string>> migrations)
        {
            
            try
            {
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    var queryList = migrations.Select(t => $"INSERT INTO {_migrationTableName} (Version) VALUES ({t.Key});{t.Value}");
                    sqLiteDatabase.OpenConnection();
                    var query = string.Join(";", queryList);
                    sqLiteDatabase.ExecuteNonQuery(query);
                    sqLiteDatabase.CloseConnection();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}