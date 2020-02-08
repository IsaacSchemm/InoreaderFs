using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sample1 {
    class TokenObject : InoreaderFs.Auth.OAuth.IAccessToken {
        public string AccessToken { get; set; }

        public override string ToString() {
            return $"{base.ToString()} ({AccessToken})";
        }
    }

    class Program {
        static async Task Main() {
            Console.Write("App ID: ");
            string appId = Console.ReadLine();
            Console.Write("App key: ");
            string appKey = Console.ReadLine();

            var app = new InoreaderFs.Auth.App(appId, appKey);

            Console.WriteLine();

            Console.WriteLine("(1) Enter existing OAuth access token");
            Console.WriteLine("(2) Acquire new OAuth access token");
            Console.WriteLine("(3) ClientLogin");
            string opt = Console.ReadLine();

            InoreaderFs.Auth.Credentials credentials = null;

            switch (opt) {
                case "1":
                    Console.Write("Access token: ");
                    string accessToken = Console.ReadLine();
                    var token1 = new TokenObject { AccessToken = accessToken };
                    credentials = InoreaderFs.Auth.Credentials.NewOAuth(token1);
                    break;
                case "2":
                    Console.Write("Redirect URI: ");
                    string redirectUri = Console.ReadLine();
                    Process.Start(
                        Environment.ExpandEnvironmentVariables(@"%PROGRAMFILES%\Internet Explorer\iexplore.exe"),
                        $"https://www.inoreader.com/oauth2/auth?client_id={Uri.EscapeDataString(app.appId)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&response_type=code&scope=read&state={Guid.NewGuid()}");
                    Console.Write("Enter \"code\" parameter from browser URL: ");
                    string code = Console.ReadLine();
                    var token2 = await InoreaderFs.Auth.OAuth.OAuthHandler.GetTokenAsync(app, code, new Uri(redirectUri));
                    credentials = InoreaderFs.Auth.Credentials.NewOAuth(token2);
                    break;
                case "3":
                    Console.Write("Email: ");
                    string email = Console.ReadLine();
                    Console.Write("Password: ");
                    string password = Console.ReadLine();
                    var auth = await InoreaderFs.Auth.ClientLogin.ClientLoginHandler.LoginAsync(email, password);
                    credentials = InoreaderFs.Auth.Credentials.NewClientLogin(app, auth);
                    break;
            }

            Console.WriteLine(credentials);

            var user = await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(credentials);
            Console.WriteLine(user);
        }
    }
}
