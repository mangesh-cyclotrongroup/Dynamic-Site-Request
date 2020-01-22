using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SiteRequest.Helpers;
using System;
using System.Configuration;
using System.IO;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace SiteRequest.Web
{
    internal class BaseHelper : KeyVaultHelper
    {
        public static string TenantName { get; set; }
        public static string AppId { get; set; }
        public static string AppSecret { get; set; }
        public static string TenantId { get; set; }
        public static string ExtensionSchemaId { get; set; }

        public override string ToString()
        {
            return $"TenantName:{TenantName} AppId:{AppId} AppSecret:{AppSecret}";
        }        

        /// <summary>
        /// BaseHelper
        /// </summary>
        static BaseHelper()
        {
            TenantName = GetVaultValue("TenantName");
            AppId = GetVaultValue("AppId");
            AppSecret = GetVaultValue("AppSecret");
            TenantId = GetVaultValue("TenantId");
            ExtensionSchemaId = GetVaultValue("ExtensionSchemaId");
        }
        public static Tuple<string,string> GetCredentials()
        {
            var userName=  GetVaultValue("UserName");
            var password = GetVaultValue("Password");

            return Tuple.Create(userName, password);
        }
   
        /// <summary>
        /// IsInValidCredentials
        /// </summary>
        /// <returns></returns>
        public static Tuple<bool, string> IsInValidCredentials()
        {
            var isInValid = false;
            StringBuilder erroeMessage = new StringBuilder();
            var vaultUrl = ConfigurationManager.AppSettings["vaultURl"];

            if (string.IsNullOrEmpty(TenantName))
            {
                isInValid = true;
                erroeMessage.Append("TenantName is empty");
            }
            if (string.IsNullOrEmpty(AppId))
            {
                isInValid = true;
                erroeMessage.Append("AppId is empty");
            }
            if (string.IsNullOrEmpty(AppSecret))
            {
                isInValid = true;
                erroeMessage.Append("AppSecret is empty");
            }
            if (string.IsNullOrEmpty(TenantId))
            {
                isInValid = true;
                erroeMessage.Append("TenantId is empty");
            }
            if (string.IsNullOrEmpty(vaultUrl))
            {
                isInValid = true;
                erroeMessage.Clear();
                erroeMessage.Append("Vault Url is empty");
                erroeMessage.Append("\n");
            }
            return Tuple.Create(isInValid, erroeMessage.ToString());
        }

        /// <summary>
        /// GetTokenAsync
        /// </summary>
        /// <returns></returns>
        internal static async Task<string> GetTokenAsync()
        {
            try
            {
                string resourceId = "https://graph.microsoft.com";
                string authString = "https://login.microsoftonline.com/" + TenantId;
                var authenticationContext = new AuthenticationContext(authString, false);
                ClientCredential clientCred = new ClientCredential(AppId, AppSecret);
                AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(resourceId, clientCred);
                return authenticationResult.AccessToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Datacontroller : " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// SetExecutionPolicy
        /// </summary>
        /// <param name="powershell"></param>
        protected static void SetExecutionPolicy(PowerShell powershell)
        {
            powershell.AddScript("Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted");
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="textToEncrypt"></param>
        /// <returns></returns>
        public static string Encrypt(string textToEncrypt)
        {
            try
            {
                string ToReturn = "";
                string _key = "ay$a5%&jwrtmnh;lasjdf98787456";
                string _iv = "abc@98797hjkas$&asd(*$%1234";
                byte[] _ivByte = { };
                _ivByte = System.Text.Encoding.UTF8.GetBytes(_iv.Substring(0, 8));
                byte[] _keybyte = { };
                _keybyte = System.Text.Encoding.UTF8.GetBytes(_key.Substring(0, 8));
                MemoryStream ms = null; CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(_keybyte, _ivByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="textToDecrypt"></param>
        /// <returns></returns>
        public static string Decrypt(string textToDecrypt)
        {
            try
            {
                string ToReturn = "";
                byte[] _ivByte = { };
                _ivByte = System.Text.Encoding.UTF8.GetBytes(Constants.RgbIV.Substring(0, 8));
                byte[] _keybyte = { };
                _keybyte = System.Text.Encoding.UTF8.GetBytes(Constants.RgbKey.Substring(0, 8));
                MemoryStream ms = null; CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(_keybyte, _ivByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }
    }
}
