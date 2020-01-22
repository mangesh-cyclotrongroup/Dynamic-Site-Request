
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;

using SiteRequest.Helpers;
using SiteRequest.Models;
using Microsoft.SharePoint.Client;
using BotDialog.Helpers;

namespace SiteRequest.Controllers
{
    [Authorize]
    public class DataController : ApiController
    {
        /// <summary>
        /// getTheme
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        [HttpGet]
        public string getTheme(string emailId)
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var dt = sqLiteDatabase.GetDataTable($"SELECT Theme,UserId FROM Theme WHERE UserId='{emailId}'");
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

        /// <summary>
        /// getTeamsHubFeatures
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string getTeamsHubFeatures()
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var dt = sqLiteDatabase.GetDataTable("SELECT * FROM TeamsHubFeatures ORDER BY DisplayOrder");
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

        /// <summary>
        /// setTeamsHubFeatures
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="subFeature"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [HttpGet]
        public string setTeamsHubFeatures(string features)
        {
            try
            {

                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    JArray array = JArray.Parse(features);
                    foreach (JObject o in array.Children<JObject>())
                    {
                        string feature = "";
                        string enabled = "";
                        string subFeature = "";

                        foreach (JProperty p in o.Properties())
                        {
                            if (p.Name == "Feature")
                            {
                                feature = (string)p.Value;
                            }
                            else if (p.Name == "SubFeature")
                            {
                                subFeature = (string)p.Value;

                            }
                            else if (p.Name == "Enabled")
                            {
                                enabled = (string)p.Value;
                            }
                        }
                        var result = sqLiteDatabase.GetDataTable($"SELECT * FROM TeamsHubFeatures Where Feature='{feature}'");
                        string query = $"UPDATE TeamsHubFeatures SET Enabled='{enabled}' WHERE Feature='{feature}'";
                        if (subFeature != "")
                        {
                            result = sqLiteDatabase.GetDataTable($"SELECT * FROM TeamsHubFeatures Where Feature='{feature}' AND SubFeature ='{subFeature}'");
                            query = $"UPDATE TeamsHubFeatures SET Enabled='{enabled}' WHERE Feature='{feature}' AND SubFeature='{subFeature}'";
                        }
                        sqLiteDatabase.ExecuteNonQuery(query);
                    }

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

        /// <summary>
        /// getTeamsInfo
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        [HttpGet]
        public string getTeamsInfo(string emailId)
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();

                    DataTable dt1 = new DataTable();
                    var p1 = dt1.Columns.Add("GroupId", typeof(string));
                    dt1.Columns.Add("Tags", typeof(string));
                    dt1.Columns.Add("isFavourite", typeof(bool));
                    dt1.Columns.Add("", typeof(bool));
                    dt1.PrimaryKey = new DataColumn[] { p1 };
                    dt1 = sqLiteDatabase.GetDataTable($"SELECT GroupId,Tags,isFavourite FROM TeamsMetaData Where UserId='{emailId}'");

                    DataTable dt2 = new DataTable();
                    var p2 = dt2.Columns.Add("GroupId", typeof(string));
                    dt2.Columns.Add("IsArchived", typeof(bool));
                    dt2.Columns.Add("IsDeleted", typeof(bool));
                    dt2.PrimaryKey = new DataColumn[] { p2 };
                    dt2 = sqLiteDatabase.GetDataTable($"SELECT Id as GroupId,IsArchived,isDeleted FROM TeamsActivity Where IsArchived='True' OR isDeleted='True' ");

                    var dt3 = dt1.Copy();
                    dt3.Merge(dt2);

                    JSONString = JsonConvert.SerializeObject(dt3);
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

        /// <summary>
        /// setPersonalTags
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="emailId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        [HttpGet]
        public string setPersonalTags(string groupId, string emailId, string tags)
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM TeamsMetaData Where UserId='{emailId}' AND groupId ='{groupId}'");
                    string query = string.Empty;
                    if (result.Rows.Count > 0)
                    {
                        query = $"UPDATE TeamsMetaData SET Tags='{tags}' WHERE UserId='{emailId}' AND GroupId='{groupId}'";
                    }
                    else
                    {
                        query = $"INSERT INTO TeamsMetaData ('UserId','GroupId','Tags') VALUES('{emailId}','{groupId}','{tags}')";
                    }
                    sqLiteDatabase.ExecuteNonQuery(query);
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

        /// <summary>
        /// getAllTags
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        public string getAllTags()
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var dt = sqLiteDatabase.GetDataTable("SELECT Id,Tags FROM Tags");
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

        /// <summary>
        /// updateTags
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        [HttpGet]
        public string updateTags(string tagId, string tags)
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM Tags Where Id='{tagId}'");
                    string query = string.Empty;
                    if (result.Rows.Count > 0)
                    {
                        var checkTag = sqLiteDatabase.GetDataTable($"SELECT * FROM Tags Where LOWER(Tags)='{tags.ToLower()}' EXCEPT SELECT * FROM Tags Where Id='{tagId}'");
                        if (checkTag.Rows.Count > 0)
                        {
                            JSONString = "ALREADY PRESENT";
                        }
                        else
                        {
                            query = $"UPDATE Tags SET Tags='{tags}' WHERE Id='{tagId}'";
                        }
                    }
                    else
                    {
                        query = $"INSERT INTO Tags ('Tags') VALUES('{tags}')";
                    }
                    sqLiteDatabase.ExecuteNonQuery(query);
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

        /// <summary>
        /// addTags
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        [HttpGet]
        public string addTags(string tags)
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM Tags Where LOWER(Tags)='{tags.ToLower()}'");
                    if (result.Rows.Count > 0)
                    {
                        JSONString = "ALREADY PRESENT";
                    }
                    else
                    {
                        sqLiteDatabase.ExecuteNonQuery($"INSERT INTO Tags ('Tags') VALUES('{tags}')");
                        sqLiteDatabase.CloseConnection();
                        JSONString = "ADDED";
                    }
                }
                return JSONString;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Datacontroller : " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// deleteTags
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        [HttpGet]
        public string deleteTags(string tagId)
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    sqLiteDatabase.ExecuteNonQuery($"DELETE FROM Tags Where Id={tagId}");
                    sqLiteDatabase.CloseConnection();
                    JSONString = "DELETED";
                }
                return JSONString;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Datacontroller : " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// addFavourites
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="emailId"></param>
        /// <returns></returns>
        [HttpGet]
        public string addFavourites(string groupId, string emailId)
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM TeamsMetaData Where UserId='{emailId}' AND groupId ='{groupId}'");
                    string query = string.Empty;
                    if (result.Rows.Count > 0)
                    {
                        query = $"UPDATE TeamsMetaData SET isFavourite=1 WHERE UserId='{emailId}' AND GroupId='{groupId}'";
                    }
                    else
                    {
                        query = $"INSERT INTO TeamsMetaData ('UserId','GroupId','isFavourite') VALUES('{emailId}','{groupId}', 1)";
                    }
                    sqLiteDatabase.ExecuteNonQuery(query);
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

        /// <summary>
        /// removeFavourites
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="emailId"></param>
        /// <returns></returns>
        [HttpGet]
        public string removeFavourites(string groupId, string emailId)
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM TeamsMetaData Where UserId='{emailId}' AND groupId ='{groupId}'");
                    string query = $"UPDATE TeamsMetaData SET isFavourite=0 WHERE UserId='{emailId}' AND GroupId='{groupId}'";
                    sqLiteDatabase.ExecuteNonQuery(query);
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
        /// <summary>
        /// getSiteProvisionConfig
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        public string getSiteProvisionConfig()
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

        /// <summary>
        /// setSiteProvisionConfig
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet]
        public string setSiteProvisionConfig(string key, string IsSiteProvisionEnabled,string ProvisionType)
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM AdminSettings Where Key='{key}'");
                    string query = string.Empty;
                    if (result.Rows.Count > 0)
                    {
                        query = $"UPDATE AdminSettings SET IsSiteProvisionEnabled='{IsSiteProvisionEnabled}', ProvisionType='{ProvisionType}' WHERE Key='{key}'";
                    }
                    else
                    {
                        query = $"INSERT INTO AdminSettings ('Key','IsSiteProvisionEnabled','ProvisionType') VALUES('{key}','{IsSiteProvisionEnabled}','{ProvisionType}')";
                    }
                    sqLiteDatabase.ExecuteNonQuery(query);
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

        /// <summary>
        /// getSiteRequest
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        public string getSiteRequest()
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var dt = sqLiteDatabase.GetDataTable("SELECT * FROM SiteProvision");
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

        /// <summary>
        /// getSiteRequestById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public string getSiteRequestById(string id)
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var dt = sqLiteDatabase.GetDataTable($"SELECT * FROM SiteProvision WHERE Id='{id}'");
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

        [HttpPut]
        public IHttpActionResult updateSiteRequest([FromBody] TeamRequest model, [FromUri]string id)
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM SiteProvision Where Id='{id}'");
                    string query = string.Empty;
                    if (result.Rows.Count > 0)
                    {
                        query = $"UPDATE SiteProvision SET Status='{model.Status}', ApprovedBy='{model.ApprovedBy}', ApprovedDate='{model.ApprovedDate}',Comments='{model.Comments}' WHERE Id='{id}'";
                        sqLiteDatabase.ExecuteNonQuery(query);
                        sqLiteDatabase.CloseConnection();
                    }
                }
                return Ok(new { Success = true, Message = "Site request updated sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.Message });
            }
        }
            
        [HttpPost]
        public IHttpActionResult SubmitSiteRequest([FromBody]TeamRequest model)
        {
            try
            {
                string JSONString = string.Empty;
                model.Name = model.Name.Replace("'", "''");
                model.Description = model.Description.Replace("'", "''");
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(m => m.Errors)
                        .Select(e => e.ErrorMessage).ToArray();
                    return BadRequest(string.Join("", errors));
                }
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM SiteProvision Where Name='{model.Name}'");
                    string query = string.Empty;
                    if (result.Rows.Count > 0)
                    {
                        JSONString = "ALREADY PRESENT";
                    }
                    else
                    {
                        query = $"INSERT INTO SiteProvision ('Name','Description', 'Alias', 'SiteType', 'Language','RequestedBy','Privacy','Classification','Owners','Members','SiteTemplateType','RequestedDate','SiteDesignTemplate','Status') VALUES('{model.Name}','{model.Description}','{model.Alias}','{model.SiteType}','{model.Language}','{model.RequestedBy}','{model.Privacy}','{model.Classification}','{model.Owners}','{model.Members}','{model.SiteTemplateType}','{model.RequestedDate}','{model.SiteDesignTemplate}','Requested')";
                    }
                    sqLiteDatabase.ExecuteNonQuery(query);
                    sqLiteDatabase.CloseConnection();
                }
                return Ok(new { Message = JSONString });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Datacontroller : " + ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult SubmitTeamRequest([FromBody]TeamRequest model)
        {
            try
            {
                string JSONString = string.Empty;
                model.Name = model.Name.Replace("'", "''");
                model.Description = model.Description.Replace("'", "''");
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(m => m.Errors)
                        .Select(e => e.ErrorMessage).ToArray();
                    return BadRequest(string.Join("", errors));
                }
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM SiteProvision Where Name='{model.Name}'");
                    string query = string.Empty;
                    if (result.Rows.Count > 0)
                    {
                        JSONString = "ALREADY PRESENT";
                        //query = $"UPDATE SiteProvision SET Description='{model.Description}' WHERE Name='{model.Name}'";
                    }
                    else
                    {
                        query = $"INSERT INTO SiteProvision ('Name','Description', 'Alias', 'SiteType', 'Language','RequestedBy','Privacy','Classification','Owners','Members','SiteTemplateType','RequestedDate','Status') VALUES('{model.Name}','{model.Description}','{model.Alias}','{model.SiteType}','{model.Language}','{model.RequestedBy}','{model.Privacy}','{model.Classification}','{model.Owners}','{model.Members}','{model.SiteTemplateType}','{model.RequestedDate}','Requested')";
                    }
                    sqLiteDatabase.ExecuteNonQuery(query);
                    sqLiteDatabase.CloseConnection();
                }
                return Ok(new { Message = JSONString });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Datacontroller : " + ex.Message);
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        public IHttpActionResult SubmitCloneTeamRequest([FromBody]TeamRequest model)
        {
            try
            {
                string JSONString = string.Empty;
                model.Name = model.Name.Replace("'", "''");
                model.Description = model.Description.Replace("'", "''");
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(m => m.Errors)
                        .Select(e => e.ErrorMessage).ToArray();
                    return BadRequest(string.Join("", errors));
                }
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM SiteProvision Where Name='{model.Name}'");
                    string query = string.Empty;
                    if (result.Rows.Count > 0)
                    {
                        JSONString = "ALREADY PRESENT";
                        //query = $"UPDATE SiteProvision SET Description='{model.Description}' WHERE Name='{model.Name}'";
                    }
                    else
                    {                        
                        query = $"INSERT INTO SiteProvision ('Name','Description', 'Alias', 'SiteType', 'Language','RequestedBy','Privacy','Classification','GroupId','IsClone','CloneParts','Owners','Members','RequestedDate','Status') VALUES('{model.Name}','{model.Description}','{model.Alias}','{model.SiteType}','{model.Language}','{model.RequestedBy}','{model.Privacy}','{model.Classification}','{model.GroupId}','{model.IsClone}','{model.CloneParts}','{model.Owners}','{model.Members}','{model.RequestedDate}','Requested')";
                    }
                    sqLiteDatabase.ExecuteNonQuery(query);
                    sqLiteDatabase.CloseConnection();
                }
                return Ok(new { Message = JSONString });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Datacontroller : " + ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// applyIRMSettingsOnLibs
        /// </summary>
        /// <param name="web"></param>
        /// <param name="clientContext"></param>
        /// <param name="restrictPermissionOnDocDownload"></param>
        /// <param name="policyTitle"></param>
        /// <param name="policyDescription"></param>
        /// <param name="allowUserToUploadDoc"></param>
        /// <param name="stopRestrict"></param>
        /// <param name="stopRestrictAt"></param>
        /// <param name="preventDocFromOpening"></param>
        /// <param name="allowViewerToPrint"></param>
        /// <param name="allowUserToRunScript"></param>
        /// <param name="allowViewerToWrite"></param>
        /// <param name="docAccessRightExp"></param>
        /// <param name="docAccessRightExpDays"></param>
        /// <param name="credsVerification"></param>
        /// <param name="credsVerificationInterval"></param>
        /// <param name="allowGroupProtection"></param>
        /// <param name="defaultGroup"></param>
        /// <returns></returns>
   
        /// <summary>
        /// getIRMSettingsFromLibs
        /// </summary>
        /// <param name="web"></param>
        /// <param name="clientContext"></param>
        /// <returns></returns>
         /// <summary>
        /// setIRMSettings
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="restrictPermissionOnDocDownload"></param>
        /// <param name="policyTitle"></param>
        /// <param name="policyDescription"></param>
        /// <param name="allowUserToUploadDoc"></param>
        /// <param name="stopRestrict"></param>
        /// <param name="stopRestrictAt"></param>
        /// <param name="preventDocFromOpening"></param>
        /// <param name="allowViewerToPrint"></param>
        /// <param name="allowUserToRunScript"></param>
        /// <param name="allowViewerToWrite"></param>
        /// <param name="docAccessRightExp"></param>
        /// <param name="docAccessRightExpDays"></param>
        /// <param name="credsVerification"></param>
        /// <param name="credsVerificationInterval"></param>
        /// <param name="allowGroupProtection"></param>
        /// <param name="defaultGroup"></param>
        /// <returns></returns>
        [HttpGet]
   
        /// <summary>
        /// getIRMSettingsTeamsLevel
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        
      
        /// <summary>
        /// setTheme
        /// </summary>
        /// <param name="emailId"></param>
        /// <param name="theme"></param>
        /// <returns></returns>
        
        public string setTheme(string emailId, string theme)
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM Theme Where UserId='{emailId}'");
                    string query = string.Empty;
                    if (result.Rows.Count > 0)
                    {
                        query = $"UPDATE THeme SET Theme='{theme}' WHERE UserId='{emailId}'";
                    }
                    else
                    {
                        query = $"INSERT INTO Theme ('UserId','Theme') VALUES('{emailId}','{theme}')";
                    }
                    sqLiteDatabase.ExecuteNonQuery(query);
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


        /// <summary>
        /// getAllTeamsActivity
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string getAllTeamsActivity()
        {
            try
            {
                string JSONString = string.Empty;
                using (var sqLiteDatabase = new SqLiteDatabase())
                {
                    sqLiteDatabase.OpenConnection();
                    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM TeamsActivity");

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Id", typeof(string));
                    dt.Columns.Add("Name", typeof(string));
                    dt.Columns.Add("Description", typeof(string));
                    dt.Columns.Add("Type", typeof(string));
                    dt.Columns.Add("Mail", typeof(string));
                    dt.Columns.Add("CreatedDate", typeof(string));
                    dt.Columns.Add("Classification", typeof(string));
                    dt.Columns.Add("MailEnabled", typeof(string));
                    dt.Columns.Add("SecurityEnabled", typeof(string));
                    dt.Columns.Add("Channelcount", typeof(int));
                    dt.Columns.Add("ChannelName", typeof(string));
                    dt.Columns.Add("Owners", typeof(string));
                    dt.Columns.Add("Members", typeof(string));
                    dt.Columns.Add("OwnersCount", typeof(int));
                    dt.Columns.Add("MembersCount", typeof(int));
                    dt.Columns.Add("Tags", typeof(string));
                    dt.Columns.Add("isArchived", typeof(string));
                    dt.Columns.Add("LastActivityDate", typeof(string));
                    dt.Columns.Add("GroupOwner", typeof(string));
                    dt.Columns.Add("ExternalMemberCount", typeof(int));
                    dt.Columns.Add("IsDeleted", typeof(string));
                    dt.Columns.Add("MailBoxEmailCount", typeof(long));
                    dt.Columns.Add("TotalFileCount", typeof(int));
                    dt.Columns.Add("TotalStorage", typeof(long));
                    dt.Columns.Add("NumberChats", typeof(long));
                    dt.Columns.Add("NumberConversations", typeof(long));
                    dt.Columns.Add("IsObselete", typeof(string));
                    dt.Columns.Add("Popular", typeof(long));
                    dt.Columns.Add("Status", typeof(string));
                    for (int rowIndex = 0; rowIndex < result.Rows.Count; rowIndex++)
                    {
                        var reader = result.Rows[rowIndex];
                        DateTime createdDate = DateTime.Parse(reader["CreatedDate"].ToString());
                        string status = "Active";
                        if (reader["isArchived"].ToString().ToLower() == "true")
                        {
                            status = "Archived";
                        }
                        else if (reader["IsDeleted"].ToString().ToLower() == "true")
                        {
                            status = "Deleted";
                        }
                        dt.Rows.Add(reader["Id"].ToString(),
                            reader["TeamsName"].ToString(),
                            reader["Description"].ToString(),
                            reader["TeamType"].ToString(),
                            reader["Mail"].ToString(),
                            createdDate.ToString("yyyy'-'MM'-'dd"),
                            reader["Classification"].ToString(),
                            reader["MailEnabled"].ToString(),
                            reader["SecurityEnabled"].ToString(),
                            Convert.ToInt32(reader["Channelcount"]),
                            reader["ChannelName"].ToString(),
                            reader["Owners"].ToString(),
                            reader["Members"].ToString(),
                            Convert.ToInt32(reader["OwnersCount"]),
                            Convert.ToInt32(reader["MembersCount"]),
                            reader["Tags"].ToString(),
                            reader["isArchived"].ToString(),
                            reader["LastActivityDate"].ToString(),
                            reader["GroupOwner"].ToString(),
                            Convert.ToInt32(reader["ExternalMemberCount"]),
                            reader["IsDeleted"].ToString(),
                            Convert.ToInt64(reader["MailBoxEmailCount"]),
                            Convert.ToInt32(reader["TotalFileCount"]),
                            Convert.ToInt64(reader["TotalStorage"]),
                            Convert.ToInt64(reader["NumberChats"]),
                            Convert.ToInt64(reader["NumberConversations"]),
                            reader["IsObselete"].ToString(),
                            Convert.ToInt64(reader["NumberConversations"]) + Convert.ToInt64(reader["NumberChats"]),
                            status);
                    }
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
      
        /// <summary>
        /// updateTeamsActivity
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
      
      
    }
}


