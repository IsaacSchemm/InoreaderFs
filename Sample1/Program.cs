using System;
using System.Threading.Tasks;

namespace Sample1 {
    class Program {
        static async Task Main() {
            var c = await InoreaderFs.Auth.ClientLogin.LoginWithAppAsync(
                new InoreaderFs.Auth.App("", ""),
                "",
                "");
            Console.WriteLine(c);

            var u = await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(c);
            Console.WriteLine(u);
        }
    }
}
