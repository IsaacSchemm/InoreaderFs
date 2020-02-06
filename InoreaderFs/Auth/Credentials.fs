namespace InoreaderFs.Auth

/// An object that stores credentials for the Inoreader API.
/// Either the OAuth or ClientLogin flows can be used.
type Credentials =
| Bearer of IBearerToken
| ClientLogin of App * ClientLoginAuth
with
    member this.Headers = seq {
        match this with
        | Bearer b ->
            ("Authorization", sprintf "Bearer %s" b.AccessToken)
        | ClientLogin (app, (ClientLoginAuth auth)) ->
            ("AppId", app.appId)
            ("AppKey", app.appKey)
            ("Authorization", sprintf "GoogleLogin auth=%s" auth)
    }