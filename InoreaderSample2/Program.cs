using InoreaderFs.Auth;
using InoreaderFs.Auth.ClientLogin;
using InoreaderFs.Auth.OAuth;
using System;
using System.Threading.Tasks;

namespace InoreaderSample2 {
    class TokenObject : IAccessToken {
        public string AccessToken { get; set; }
    }

    class Program {
        static async Task<Credentials> GetCredentialsAsync() {
            Console.WriteLine("(1) OAuth access token");
            Console.WriteLine("(2) ClientLogin");
            string opt = Console.ReadLine();

            switch (opt) {
                case "1":
                    Console.Write("Access token: ");
                    string accessToken = Console.ReadLine();
                    var tokenObj = new TokenObject { AccessToken = accessToken };
                    return Credentials.NewOAuth(tokenObj);
                case "2":
                    Console.Write("App ID: ");
                    string appId = Console.ReadLine();
                    Console.Write("App key: ");
                    string appKey = Console.ReadLine();
                    var app = new App(appId, appKey);

                    Console.Write("Email: ");
                    string email = Console.ReadLine();
                    Console.Write("Password: ");
                    string password = Console.ReadLine();
                    var auth = await ClientLoginHandler.LoginAsync(email, password);

                    return Credentials.NewClientLogin(app, auth);
                default:
                    throw new Exception("Invalid number entered");
            }
        }

        static async Task Main() {
            Credentials credentials = await GetCredentialsAsync();
            Console.WriteLine(credentials);

            var user = await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(credentials);
            Console.WriteLine(user);
        }
    }
}
