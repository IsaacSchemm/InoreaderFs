using System;
using System.Threading.Tasks;

namespace Sample1 {
    class Program {
        static async Task Main() {
            string username = "";
            string password = "";

            string s = await InoreaderFs.ClientLogin.LoginAsync(username, password);
            Console.WriteLine(s);

            var app = new InoreaderFs.Authentication.AppCredentials("", "");
            var c = new InoreaderFs.Authentication.ClientLoginCredentials(app, s);
            var u = await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(c);
            Console.WriteLine(u);
        }
    }
}
