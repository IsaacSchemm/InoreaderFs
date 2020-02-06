namespace InoreaderFs

open System
open System.Net
open System.IO
open FSharp.Json

type IRefreshToken =
    inherit IBearerToken
    abstract member RefreshToken: string

type IRefreshTokenFull =
    inherit IRefreshToken
    abstract member ExpiresAt: DateTimeOffset
    abstract member Scopes: string seq

type IOAuth =
    abstract member AsyncRefresh: string -> Async<IRefreshTokenFull>

type TokenResponse = {
  access_token: string
  token_type: string
  expires_in: int
  refresh_token: string
  scope: string
} with
    interface IRefreshTokenFull with
        member this.AccessToken = this.access_token
        member this.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds (float this.expires_in)
        member this.RefreshToken = this.refresh_token
        member this.Scopes = this.scope.Split(' ') :> seq<string>

type OAuth(app: App) =
    let UserAgent = "InoreaderFs/0.0 (https://github.com/IsaacSchemm/InoreaderFs)"

    let BuildForm (dict: seq<string * string>) =
        let parameters = seq {
            for k, v in dict do
                if isNull v then
                    failwithf "Null values in form not allowed"
                let key = Uri.EscapeDataString k
                let value = Uri.EscapeDataString v
                yield sprintf "%s=%s" key value
        }
        String.concat "&" parameters

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
                |> BuildForm
                |> sw.WriteAsync
                |> Async.AwaitTask
        }

        use! resp = req.AsyncGetResponse()
        use sr = new StreamReader(resp.GetResponseStream())
        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        let obj = Json.deserialize<TokenResponse> json
        if obj.token_type <> "Bearer" then
            failwithf "token_type was not Bearer"
        return obj :> IRefreshTokenFull
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
                |> BuildForm
                |> sw.WriteAsync
                |> Async.AwaitTask
        }

        use! resp = req.AsyncGetResponse()
        use sr = new StreamReader(resp.GetResponseStream())
        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        let obj = Json.deserialize<TokenResponse> json
        if obj.token_type <> "Bearer" then
            failwithf "token_type was not Bearer"
        return obj :> IRefreshTokenFull
    }

    member this.GetTokenAsync code redirect_uri =
        this.AsyncGetToken code redirect_uri |> Async.StartAsTask
    member this.RefreshAsync refresh_token =
        this.AsyncRefresh refresh_token |> Async.StartAsTask

    interface IOAuth with
        member this.AsyncRefresh refresh_token = this.AsyncRefresh refresh_token