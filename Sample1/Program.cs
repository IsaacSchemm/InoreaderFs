using System;
using System.Threading.Tasks;

namespace Sample1 {
    class Program {
        static async Task Main() {
            var a = new InoreaderFs.Auth.App("", "");
            var l = await InoreaderFs.Auth.ClientLogin.LoginAsync("", "");
            var c = InoreaderFs.Auth.Credentials.NewClientLogin(a, l);
            Console.WriteLine(c);

            var u = await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(c);
            Console.WriteLine(u);
        }
    }
}
