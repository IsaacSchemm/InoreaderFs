using InoreaderFs.Auth;
using InoreaderFs.Auth.ClientLogin;
using System;
using System.Threading.Tasks;

namespace InoreaderSample2 {
    class Program {
        static async Task Main() {
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

            var credentials = Credentials.NewClientLogin(app, auth);

            Console.WriteLine(credentials);

            var user = await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(credentials);
            Console.WriteLine(user);
        }
    }
}
