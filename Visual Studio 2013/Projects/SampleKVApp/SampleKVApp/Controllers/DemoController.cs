using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace SampleKVApp.Controllers
{
    public class DemoController : Controller  
{  
  public ActionResult Index()  
  {  
    var vaultAddress = WebConfigurationManager.AppSettings["VaultUrl"];  
  
    // Register authentication call back - this would be executed for any request to Azure key vault.  
              
    KeyVaultClient keyVaultClient = new KeyVaultClient(new        KeyVaultClient.AuthenticationCallback(GetAccessToken));  
  
   var secret = keyVaultClient.GetSecretAsync(vaultAddress, "BhushanDemoStoragePrimaryKey", null).GetAwaiter().GetResult();  
  
   var storagePrimaryAccessKey = secret.Value;  
  
   return View();  
  }  
  
  public static async Task<string> GetAccessToken(string authority, string resource, string scope)  
  {  
    var clientId = WebConfigurationManager.AppSettings["AuthClientId"];  
    var clientSecret = WebConfigurationManager.AppSettings["AuthClientSecret"];  
    ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);  
  
    var context = new AuthenticationContext(authority, TokenCache.DefaultShared);  
    var result = await context.AcquireTokenAsync(resource, clientCredential);  
  
    return result.AccessToken;  
  }  
 }  
}  
}
