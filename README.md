# InoreaderFs

An F# / .NET library to access the [Inoreader API.](https://www.inoreader.com/developers/)

Supported endpoints:

* User information
* Add subscription
* Edit subscription
* Unread counts
* Subscription list
* Folder/Tag list
* Stream contents
* Item IDs
* Stream preferences list
* Stream preferences set
* Rename tag
* Delete tag
* Edit tag
* Mark all as read

Unsupported endpoints:
* Create active search
* Delete active search

## Authentication

Both OAuth 2.0 and ClientLogin are supported.
(If you use OAuth, you'll have to handle the redirect leg yourself.)

Since two types of authentication are supported, InoreaderFs uses a
[discriminated union](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/discriminated-unions)
as the first parameter to the static methods in the `InoreaderFs.Endpoints`
namespace that allow access to the API. The `InoreaderFs.Auth.Credentials`
union has two cases:

* **OAuth** - wraps an `InoreaderFs.Auth.OAuth.IAccessToken`
* **ClientLogin** - wraps an `InoreaderFs.Auth.App` and a string acquired from
  `InoreaderFs.Auth.ClientLogin.ClientLoginHandler`

To use OAuth, you'll need to implement the `IAccessToken` interface yourself.
However, you can take advantage of the opportunity to also implement
`InoreaderFs.Auth.OAuth.IAutoRefreshToken`, which will let InoreaderFs refresh
your tokens automatically for you when they expire.

### OAuth (access token only)

	class InoreaderToken : InoreaderFs.Auth.OAuth.IAccessToken {
		public string AccessToken { get; set; }
	}

	static async Task Main() {
		var token = new InoreaderToken { AccessToken = "your_access_token_here" };
		var credentials = InoreaderFs.Auth.Credentials.NewOAuth(token);

		var user = await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(credentials);
		Console.WriteLine(user);
	}

### OAuth (handle refresh tokens automatically)

	class TokenWrapper : InoreaderFs.Auth.OAuth.IAutoRefreshToken {
		public App App => new InoreaderFs.Auth.App("app ID", "app key");

		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }

		public async Task UpdateTokenAsync(IRefreshToken newToken) {
			this.AccessToken = newtoken.AccessToken;
			this.RefreshToken = newtoken.RefreshToken;
			await YourBackingStore.UpdateTokensAsync(this.AccessToken, this.RefreshToken);
		}
	}

	static async Task Main() {
		var wrapper = new TokenWrapper();
		(this.AccessToken, this.RefreshToken) = await YourBackingStore.GetTokensAsync();

		var credentials = InoreaderFs.Auth.Credentials.NewOAuth(wrapper);

		var user = await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(credentials);
		Console.WriteLine(user);
	}

### ClientLogin (deprecated)

	static async Task Main() {
		var app = new InoreaderFs.Auth.App("app ID", "app key");
		var auth = await InoreaderFs.Auth.ClientLogin.ClientLoginHandler.LoginAsync("email", "password");

		var credentials = InoreaderFs.Auth.Credentials.NewClientLogin(app, auith);

		var user = await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(credentials);
		Console.WriteLine(user);
	}
