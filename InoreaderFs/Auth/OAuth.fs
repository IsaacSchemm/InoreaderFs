namespace InoreaderFs.Auth

open System
open System.Net
open System.IO
open FSharp.Json
open InoreaderFs

/// An object that has an access token for the Inoreader API.
type IBearerToken =
    abstract member AccessToken: string

/// An object that has access and refresh tokens for the Inoreader API.
type IRefreshToken =
    inherit IBearerToken
    abstract member RefreshToken: string

/// An Inoreader API token returned from the OAuth2 "token" endpoint.
type RefreshToken = {
  access_token: string
  token_type: string
  expires_in: int
  refresh_token: string
  scope: string
} with
    interface IRefreshToken with
        member this.AccessToken = this.access_token
        member this.RefreshToken = this.refresh_token

/// A module for getting Inoreader API tokens via OAuth2.
module OAuth =
    /// Gets a token from the server, given a code from the OAuth2 authorization code flow.
    let AsyncGetToken (app: App) (code: string) (redirect_uri: Uri) = async {
        if isNull code then
            nullArg "code"
        if isNull redirect_uri then
            nullArg "redirect_uri"

        let req = WebRequest.CreateHttp "https://www.inoreader.com/oauth2/token"
        req.UserAgent <- Shared.UserAgent
        req.Method <- "POST"
        req.ContentType <- "application/x-www-form-urlencoded"

        do! async {
            use! reqStream = req.GetRequestStreamAsync() |> Async.AwaitTask
            use sw = new StreamWriter(reqStream)
            do!
                seq {
                    ("client_id", app.appId)
                    ("client_secret", app.appKey)
                    ("grant_type", "authorization_code")
                    ("code", code)
                    ("redirect_uri", redirect_uri.AbsoluteUri)
                }
                |> Shared.BuildForm
                |> sw.WriteAsync
                |> Async.AwaitTask
        }

        use! resp = req.AsyncGetResponse()
        use sr = new StreamReader(resp.GetResponseStream())
        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        let obj = Json.deserialize<RefreshToken> json
        if obj.token_type <> "Bearer" then
            failwithf "token_type was not Bearer"
        return obj
    }

    /// Gets a token from the server, given a code from the OAuth2 authorization code flow.
    let GetTokenAsync app code redirect_uri =
        AsyncGetToken app code redirect_uri |> Async.StartAsTask

    /// Uses a refresh token to get a new set of tokens from the server.
    let AsyncRefresh (app: App) (refresh_token: string) = async {
        if isNull refresh_token then
            nullArg "refresh_token"

        let req = WebRequest.CreateHttp "https://www.inoreader.com/oauth2/token"
        req.UserAgent <- Shared.UserAgent
        req.Method <- "POST"
        req.ContentType <- "application/x-www-form-urlencoded"
            
        do! async {
            use! reqStream = req.GetRequestStreamAsync() |> Async.AwaitTask
            use sw = new StreamWriter(reqStream)
            do!
                seq {
                    ("client_id", app.appId)
                    ("client_secret", app.appKey)
                    ("grant_type", "refresh_token")
                    ("refresh_token", refresh_token)
                }
                |> Shared.BuildForm
                |> sw.WriteAsync
                |> Async.AwaitTask
        }

        use! resp = req.AsyncGetResponse()
        use sr = new StreamReader(resp.GetResponseStream())
        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        let obj = Json.deserialize<RefreshToken> json
        if obj.token_type <> "Bearer" then
            failwithf "token_type was not Bearer"
        return obj
    }

    /// Uses a refresh token to get a new set of tokens from the server.
    let RefreshAsync app refresh_token =
        AsyncRefresh app refresh_token |> Async.StartAsTask