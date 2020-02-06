namespace InoreaderFs.Authentication

type ClientLoginCredentials = {
    app: AppCredentials
    auth: string
} with
    interface IInoreaderCredentials with
        member this.GetHeaders() = seq {
            ("AppId", this.app.appId)
            ("AppKey", this.app.appKey)
            ("Authorization", sprintf "GoogleLogin auth=%s" this.auth)
        }