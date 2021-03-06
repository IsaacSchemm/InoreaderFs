﻿namespace InoreaderFs.Auth

open InoreaderFs.Auth.OAuth

/// An object that stores credentials for the Inoreader API.
/// Either the OAuth or ClientLogin flows can be used.
type Credentials =
| OAuth of IAccessToken
| ClientLogin of App * string
with
    member this.Headers = seq {
        match this with
        | OAuth b ->
            ("Authorization", sprintf "Bearer %s" b.AccessToken)
        | ClientLogin (app, auth) ->
            ("AppId", app.appId)
            ("AppKey", app.appKey)
            ("Authorization", sprintf "GoogleLogin auth=%s" auth)
    }