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

	static class YourBackend {
		Task<InoReaderToken> LoadTokensAsync();
		Task SaveTokensAsync(InoReaderToken token);
	}

	class InoreaderToken : InoreaderFs.Auth.OAuth.IAutoRefreshToken {
		public App App => new InoreaderFs.Auth.App("app ID", "app key");

		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }

		public async Task UpdateTokenAsync(IRefreshToken newToken) {
			this.AccessToken = newtoken.AccessToken;
			this.RefreshToken = newtoken.RefreshToken;
			await YourBackend.SaveTokensAsync(this);
		}
	}

	static async Task Main() {
		var token = await YourBackend.LoadTokensAsync(token);

		var credentials = InoreaderFs.Auth.Credentials.NewOAuth(token);

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
