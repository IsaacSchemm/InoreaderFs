namespace InoreaderFs

type App = {
    appId: string
    appKey: string
}

type ClientLoginCredentials = {
    app: App
    auth: string
}

type IBearerToken =
    abstract member AccessToken: string

type Credentials =
| ClientLogin of ClientLoginCredentials
| Bearer of IBearerToken
with
    member this.Headers = seq {
        match this with
        | ClientLogin c ->
            ("AppId", c.app.appId)
            ("AppKey", c.app.appKey)
            ("Authorization", sprintf "GoogleLogin auth=%s" c.auth)
        | Bearer b ->
            ("Authorization", sprintf "Bearer %s" b.AccessToken)
    }