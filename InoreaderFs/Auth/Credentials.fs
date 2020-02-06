namespace InoreaderFs.Auth

type Credentials =
| ClientLogin of App * ClientLoginAuth
| Bearer of IBearerToken
with
    member this.Headers = seq {
        match this with
        | ClientLogin (app, (ClientLoginAuth auth)) ->
            ("AppId", app.appId)
            ("AppKey", app.appKey)
            ("Authorization", sprintf "GoogleLogin auth=%s" auth)
        | Bearer b ->
            ("Authorization", sprintf "Bearer %s" b.AccessToken)
    }