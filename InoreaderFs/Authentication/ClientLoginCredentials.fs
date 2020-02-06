namespace InoreaderFs.Authentication

type ClientLoginCredentials = {
    appId: string
    appKey: string
    auth: string
} with
    interface IInoreaderCredentials with
        member this.GetHeaders() = dict ([
            ("AppId", this.appId)
            ("AppKey", this.appKey)
            ("Authorization", sprintf "GoogleLogin auth=%s" this.auth)
        ])