//////using System;
//////using System.Collections.Generic;
//////using System.Linq;
//////using System.Text;
//////using System.Threading.Tasks;
//////using Google.Apis.Drive.v1;
//////using Google.Apis.Drive.v1.Data;
//////using System.IO;
//////using Google.Apis.Auth.OAuth2;
//////using System.Threading;
//////using Google.Apis.Util.Store;
//////using System.Security.Cryptography.X509Certificates;
//////public class GDriveAuthentication
//////{
//////    public DriveService GetDriveService()
//////    {
//////        string CLIENT_ID = "CLIENT ID";
//////        string CLIENT_SECRET = "CLIENT SECRET";

//////        // Register the authenticator and create the service
//////        var provider = new NativeApplicationClient(
//////            GoogleAuthenticationServer.Description, CLIENT_ID, CLIENT_SECRET);
//////        var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
//////        var service = new DriveService(new BaseClientService.Initializer()
//////        {
//////            Authenticator = auth
//////        });

//////        return service;
//////    }

//////    private IAuthorizationState GetAuthorization(NativeApplicationClient arg)
//////    {
//////        // Get the auth URL:
//////        IAuthorizationState state =
//////          new AuthorizationState(new[] { DriveService.Scopes.Drive.GetStringValue() });
//////        state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
//////        Uri authUri = arg.RequestUserAuthorization(state);

//////        // Request authorization from the user (by opening a browser window):

//////        //Process.Start(authUri.ToString());
//////        //Console.Write("  Authorization Code: ");
//////        //string authCode = Console.ReadLine();
//////        //Console.WriteLine();

//////        var authCode = AuthorizationWindow.GetToken(authUri.ToString());

//////        if (string.IsNullOrEmpty(authCode))
//////            authCode = Interaction.InputBox("We did not find authcode. " +
//////              "Please enter here to continue", "Authorization Code", string.Empty);

//////        // Retrieve the access token by using the authorization code:
//////        return arg.ProcessUserAuthorization(authCode ?? string.Empty, state);
//////    }
//////}