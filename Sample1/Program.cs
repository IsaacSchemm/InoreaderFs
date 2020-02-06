using System;
using System.Threading.Tasks;

namespace Sample1 {
    class Program {
        static async Task Main() {
            var c = await InoreaderFs.ClientLogin.LoginWithAppAsync(
                new InoreaderFs.App("", ""),
                "",
                "");
            Console.WriteLine(c);

            var u = await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(c);
            Console.WriteLine(u);
        }
    }
}
