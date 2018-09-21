using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.IO;
using System.Configuration;

namespace StorageReplicationService
{
    public partial class StorageReplication : ServiceBase
    {
        public static ClientAssertionCertificate AssertionCert { get; set; }
        const string EVENT_SOURCE = "StorageReplicationService";
        string sLog = "StorageReplicationService";
        
        public StorageReplication()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
          //  System.Diagnostics.Debugger.Launch();
            try
            {
                if (!EventLog.SourceExists(EVENT_SOURCE))
                    EventLog.CreateEventSource(EVENT_SOURCE, sLog);
                EventLog.WriteEntry(EVENT_SOURCE, "StorageReplicationService Service Started " + DateTime.Now.ToString());

                if (!string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["vaultAddress"])) &&
                    !string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["PrimaryStorage"])) &&
                    !string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["SecondaryStorage"])))
                {
                    var vaultAddress = Convert.ToString(ConfigurationManager.AppSettings["vaultAddress"]);

                    CertificateHelper.GetCert(); //Retrive certificate information
                    KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));

                    var SourceKey = keyVaultClient.GetSecretAsync(vaultAddress, "PrimaryStorageKey").GetAwaiter().GetResult();
                    var PrimaryStorageKey = SourceKey.Value;

                    var DestKey = keyVaultClient.GetSecretAsync(vaultAddress, "BackupStorageKey").GetAwaiter().GetResult();
                    var BackupStorageKey = DestKey.Value;

                    Process storagebkp = new Process();

                    storagebkp.StartInfo.FileName = "cmd.exe";

                    storagebkp.StartInfo.Arguments = "/C AzCopy.exe /Source:" + Convert.ToString(ConfigurationManager.AppSettings["PrimaryStorage"]) + Convert.ToString(ConfigurationManager.AppSettings["SecondaryStorage"]) + " /SourceKey:" + PrimaryStorageKey + " /DestKey:" + BackupStorageKey + " /S /XO /V:" + Convert.ToString(ConfigurationManager.AppSettings["LogFileLocation"]) + "Log_StorageBkp_gorillaprodcdn" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + ".log";

                    storagebkp.Start();
                }

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(EVENT_SOURCE, "Exception occured in StorageReplicationService OnStart :" + ex.ToString() );
            }

        }

        public static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            try
            {
                var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
                var result = await context.AcquireTokenAsync(resource, AssertionCert);
                return result.AccessToken;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(EVENT_SOURCE, "Exception occured in StorageReplicationService GetAccessToken:" + ex.ToString());
                return null;
            }

           


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
                catch (Exception ex)
                {
                    EventLog.WriteEntry(EVENT_SOURCE, "Exception occured in StorageReplicationService FindCertificateByThumbprint:" + ex.ToString());
                    return null;
                }
                finally
                {
                    store.Close();
                }

            }

            public static void GetCert()
            {
                try {
                    if (!string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["CertificateThumbprint"])) &&
                  !string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["ClientID"])))
                    {
                        var clientAssertionCertPfx = FindCertificateByThumbprint(CleanThumbprint(Convert.ToString(ConfigurationManager.AppSettings["CertificateThumbprint"])));
                        AssertionCert = new ClientAssertionCertificate(Convert.ToString(ConfigurationManager.AppSettings["ClientID"]), clientAssertionCertPfx);
                    }
                   
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry(EVENT_SOURCE, "Exception occured in StorageReplicationService GetCert:" + ex.ToString());
                    
                }

            }
        }


        public static string CleanThumbprint(string mmcThumbprint)
        {
            //replace spaces, non word chars and convert to uppercase
            return Regex.Replace(mmcThumbprint, @"\s|\W", "").ToUpper();
        }
        protected override void OnStop()
        {
            try { 
            if (!EventLog.SourceExists(EVENT_SOURCE))
                EventLog.CreateEventSource(EVENT_SOURCE, sLog);
            EventLog.WriteEntry(EVENT_SOURCE, "StorageReplicationService Service Stopped " + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(EVENT_SOURCE, "Exception occured in StorageReplicationService OnStop:" + ex.ToString());

            }

        }
    }
}
