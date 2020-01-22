using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace SiteRequest.Helpers
{
    public class KeyVaultHelper
    {
        /// <summary>
        /// GetVaultValue
        /// </summary>
        /// <param name="vaultUrl"></param>
        /// <param name="keyVaultClient"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        protected static string GetVaultValue(string keyName)
        {
            try
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                var vaultUrl = ConfigurationManager.AppSettings["VaultUrl"];
                var secret = keyVaultClient.GetSecretAsync(vaultUrl, keyName).GetAwaiter().GetResult();
                return secret?.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {DateTime.Now} while Getting {keyName} from key vault : { ex.ToString()}");
                return null;
            }
        }


        /// <summary>
        /// setVaultValue
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public static async Task setVaultValue(string keyName, string keyValue)
        {
            try
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                var vaultUrl = ConfigurationManager.AppSettings["VaultUrl"];
                await keyVaultClient.SetSecretAsync(vaultUrl, keyName, keyValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {DateTime.Now} while Getting {keyName} from key vault : { ex.ToString()}");
            }
        }

    }
}