using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.IO;

namespace StorageBackupService
{
    class Program
    {
        public static ClientAssertionCertificate AssertionCert { get; set; }
        static void Main(string[] args)
        {
            //Address of the vault where secret will be stored
            var vaultAddress = "https://gorilla-app-qc-key-vault.vault.azure.net:443";
            CertificateHelper.GetCert(); //Retrive certificate information
            KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));

            var SourceKey = keyVaultClient.GetSecretAsync(vaultAddress, "PrimaryStorageKey").GetAwaiter().GetResult();
            var PrimaryStorageKey = SourceKey.Value;

            var DestKey = keyVaultClient.GetSecretAsync(vaultAddress, "BackupStorageKey").GetAwaiter().GetResult();
            var BackupStorageKey = DestKey.Value;

            Process storagebkp = new Process();

            storagebkp.StartInfo.FileName = "cmd.exe";
            storagebkp.StartInfo.Arguments = "/C AzCopy.exe /Source:https://gorillaprodcdn.blob.core.windows.net/cdn /Dest:https://gorillaprodcdndr.blob.core.windows.net/cdn /SourceKey:" + PrimaryStorageKey + " /DestKey:" + BackupStorageKey + " /S /XO /V:C:\\log\\Log_StorageBkp_gorillaprodcdn201701133.log";

            storagebkp.Start();

            Console.ReadLine();
        }


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
                var clientAssertionCertPfx = FindCertificateByThumbprint(CleanThumbprint("‎‎c50d923d51afe25960cd570cb39221354d79eee4"));
                AssertionCert = new ClientAssertionCertificate("7f8e800f-c4dd-43ac-af46-fe80daecefe0", clientAssertionCertPfx);
            }
        }


        public static string CleanThumbprint(string mmcThumbprint)
        {
            //replace spaces, non word chars and convert to uppercase
            return Regex.Replace(mmcThumbprint, @"\s|\W", "").ToUpper();
        }
    }


}
