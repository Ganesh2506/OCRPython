using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure.KeyVault;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Cryptography.X509Certificates;


namespace KeyVaultDemo.Controllers
{
    
//    public class HomeController : Controller
//    {
//        public ActionResult Index()
//        {
//            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
//             string VaultUrl = "https://dbvault.vault.azure.net/";
//string ClientId = "ade04c06-fb1e-4844-8fc2-f6de568c431b";
//string ApplicationKey = "GaneshKDbVault";
// string SecretName = "DbConnectionString";
// KeyVaultClient VaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));


//            return View();
//        }

//  //    public ActionResult Index()  
//  //{  
//  //  var vaultAddress = WebConfigurationManager.AppSettings["VaultUrl"];  
  
//  //  // Register authentication call back - this would be executed for any request to Azure key vault.  
              
//  //  KeyVaultClient keyVaultClient = new KeyVaultClient(new        KeyVaultClient.AuthenticationCallback(GetAccessToken));  
  
//  // var secret = keyVaultClient.GetSecretAsync(vaultAddress, "BhushanDemoStoragePrimaryKey", null).GetAwaiter().GetResult();  
  
//  // var storagePrimaryAccessKey = secret.Value;  
  
//  // return View();  
//  //}  

//        public static async Task<string> GetAccessToken(string authority, string resource, string scope)
//        {
//            var clientId = WebConfigurationManager.AppSettings["AuthClientId"];
//            var clientSecret = WebConfigurationManager.AppSettings["AuthClientSecret"];
//            ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);

//            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
//            var result = await context.AcquireTokenAsync(resource, clientCredential);

//            return result.AccessToken;
//        }  
// //}  

//        private static async Task<string> GetToken(string authority, string resource, string ClientId)
//        {
//            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
//            var authResult = await context.AcquireTokenAsync(resource, ClientId, new UserCredential());
//            return authResult.AccessToken;
//        }
//        public ActionResult About()
//        {
//            ViewBag.Message = "Your app description page.";

//            return View();
//        }

//        public ActionResult Contact()
//        {
//            ViewBag.Message = "Your contact page.";

//            return View();
//        }
//    }

    public class HomeController : Controller  
{
        public static ClientAssertionCertificate AssertionCert { get; set; }
  public ActionResult Index()  
  {  
      //Address of the vault where secret will be stored
      var vaultAddress = "https://gorilla-app-qc-key-vault.vault.azure.net:443";  
  
    // Register authentication call back - this would be executed for any request to Azure key vault.  
    CertificateHelper.GetCert(); //Retrive certificate information
    KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));

    var secret = keyVaultClient.GetSecretAsync(vaultAddress, "Gorilla-apps-EncryptionKey-Secret").GetAwaiter().GetResult();  
  
   var storagePrimaryAccessKey = secret.Value;
   ViewBag.Message = secret.Value;
   return View();  
  }  
  
        /// <summary>
        /// For authentication with clientid and secretkey
        /// </summary>
        /// <param name="authority"></param>
        /// <param name="resource"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
  //public static async Task<string> GetAccessToken(string authority, string resource, string scope)  
  //{  
  //  var clientId = "ade04c06-fb1e-4844-8fc2-f6de568c431b";  
  //  var clientSecret = "lLQS7LSUgqpiNyyM4MJLY3PGu5qtH4gUE/i8k+EaG8M="; //Key ID 
  //  ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);  
  
  //  var context = new AuthenticationContext(authority, TokenCache.DefaultShared);  
  //  var result = await context.AcquireTokenAsync(resource, clientCredential);  
  
  //  return result.AccessToken;  
  //}

  //To authenticate using ClientID and Certificate
  public static async Task<string> GetAccessToken(string authority, string resource, string scope)
  {

      var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
      var result = await context.AcquireTokenAsync(resource, AssertionCert);

      return result.AccessToken;


  }
        //Will be a seperate class
  public static class CertificateHelper
  {
      public static X509Certificate2 FindCertificateByThumbprint(string findValue)
      {
          X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
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
         // var clientAssertionCertPfx = FindCertificateByThumbprint("339F574579605A9D4F06636639E19CB0FF99B14E");
          //AssertionCert = new ClientAssertionCertificate("34617285-6f88-4608-9e9e-97157c60126a", clientAssertionCertPfx);

          var clientAssertionCertPfx = FindCertificateByThumbprint("C50D923D51AFE25960CD570CB39221354D79EEE4");
          AssertionCert = new ClientAssertionCertificate("df4b97dc-7b8d-44f9-8b93-e0309c51ba03", clientAssertionCertPfx);
      }
  }
 }  
}  

