using System;
using System.Threading.Tasks;

namespace Sample1 {
    class Program {
        static async Task Main(string[] args) {
            Console.Write("Username:");
            string username = Console.ReadLine();
            Console.Write("Password:");
            string password = Console.ReadLine();

            string s = await InoreaderFs.ClientLogin.LoginAsync(username, password);
            Console.WriteLine(s);

            string u = await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(new InoreaderFs.Authentication.ClientLoginCredentials("999999863", "lt4J7WXD1LH3P127sC7XS7cDmiaF2lkq", s));
            Console.WriteLine(u);
        }
    }
}
