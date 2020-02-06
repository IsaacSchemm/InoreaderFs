namespace InoreaderFs.Auth

open System
open System.Net
open System.IO
open FSharp.Json
open InoreaderFs

type IBearerToken =
    abstract member AccessToken: string

type IRefreshToken =
    inherit IBearerToken
    abstract member RefreshToken: string

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

type OAuth(app: App) =
    let UserAgent = "InoreaderFs/0.0 (https://github.com/IsaacSchemm/InoreaderFs)"

    member __.App = app

    member __.AsyncGetToken (code: string) (redirect_uri: Uri) = async {
        if isNull code then
            nullArg "code"
        if isNull redirect_uri then
            nullArg "redirect_uri"

        let req = WebRequest.CreateHttp "https://www.inoreader.com/oauth2/token"
        req.UserAgent <- UserAgent
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
                |> QueryStringBuilder.BuildForm
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

    member __.AsyncRefresh (refresh_token: string) = async {
        if isNull refresh_token then
            nullArg "refresh_token"

        let req = WebRequest.CreateHttp "https://www.inoreader.com/oauth2/token"
        req.UserAgent <- UserAgent
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
                |> QueryStringBuilder.BuildForm
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

    member this.GetTokenAsync code redirect_uri =
        this.AsyncGetToken code redirect_uri |> Async.StartAsTask
    member this.RefreshAsync refresh_token =
        this.AsyncRefresh refresh_token |> Async.StartAsTask