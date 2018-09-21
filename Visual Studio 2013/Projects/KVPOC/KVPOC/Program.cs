using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.IO;


namespace KVPOC
{
    public class Program
    {

        public static ClientAssertionCertificate AssertionCert { get; set; }
        static void Main(string[] args)
        {
            try
            {
                //Address of the vault where secret will be stored
                var vaultAddress = "https://gorilla-app-qc-key-vault.vault.azure.net:443";

                // Register authentication call back - this would be executed for any request to Azure key vault.
                CertificateHelper.GetCert(); //Retrive certificate information
                KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
                Console.WriteLine("Reading Value of key 'BckupStorageKey' from vault : ");

                //without version, always reads latest value
                var secret = keyVaultClient.GetSecretAsync(vaultAddress, "BackupStorageKey").GetAwaiter().GetResult();
                var storagePrimaryAccessKey = secret.Value;
                Console.WriteLine(storagePrimaryAccessKey);

                //Console.WriteLine("Reading Value of key 'DbConnectionString' from vault : ");
                ////Using version of secret
                //secret = keyVaultClient.GetSecretAsync(vaultAddress, "DbConnectionString", "91d56ec1dc5141eea3f0bc2794e3b689").GetAwaiter().GetResult();
                //storagePrimaryAccessKey = secret.Value;
                //Console.WriteLine(storagePrimaryAccessKey);

                Console.ReadLine();
            }
            catch (AdalException ex)
            { 
            
            }

        }

        //To authenticate using ClientID and Secretkey id
        //public static async Task<string> GetAccessToken(string authority, string resource, string scope)
        //{
        //    var clientId = "ade04c06-fb1e-4844-8fc2-f6de568c431b";
           
        //    var clientSecret = "BpFNBqqHskeC5+EAM0QFh8rnUzgjlEEU8A+Nn8rOrXQ="; //Key ID 
        //    ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);

        //    var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
        //    var result = await context.AcquireTokenAsync(resource, clientCredential);

        //    return result.AccessToken;
        //}

        //To authenticate using ClientID and Certificate
        public static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            
                var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
                var result = await context.AcquireTokenAsync(resource, AssertionCert);
                
                return result.AccessToken;

                
        }

        public static class CertificateHelper
        {
            public static X509Certificate2 FindCertificateByThumbprint(string findValue)
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                try
                {
                    store.Open(OpenFlags.ReadOnly);
                    X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindByThumbprint,
                        findValue, false); // Don't validate certs, since the test root isn't installed.
                    if (col == null || col.Count == 0)
                        return null;
                    return col[0];
                }
                finally
                {
                    store.Close();
                }

            }

            public static void GetCert()
            {
                var clientAssertionCertPfx = FindCertificateByThumbprint("c880c14f5c831f05caedff46488bf1d9a95f1430");
                AssertionCert = new ClientAssertionCertificate("7f8e800f-c4dd-43ac-af46-fe80daecefe0", clientAssertionCertPfx);
            }
        }


      
    }
}
