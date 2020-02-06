namespace InoreaderFs

open System
open System.Net
open InoreaderFs.Auth

type InoreaderRequest(path: string) =
    let InoreaderUri = new Uri("https://www.inoreader.com/")

    let Is403 (response: WebResponse) =
        match response with
        | :? HttpWebResponse as r -> r.StatusCode = HttpStatusCode.Forbidden
        | _ -> false

    member __.CreateRequest (credentials: Credentials) =
        let req = new Uri(InoreaderUri, path) |> WebRequest.CreateHttp
        for (k, v) in credentials.Headers do
            req.Headers.[k] <- v
        req

    member this.AsyncGetResponse (credentials: Credentials) = async {
        let req = new Uri(InoreaderUri, path) |> WebRequest.CreateHttp
        for (k, v) in credentials.Headers do
            req.Headers.[k] <- v
        try
            return! req.AsyncGetResponse()
        with
            | :? WebException as ex when Is403 ex.Response ->
                match credentials with
                | Bearer (:? IAutoRefreshToken as auto) ->
                    do! TokenTools.AsyncRefresh auto
                    let newToken = TokenTools.NoRefresh auto
                    return! this.AsyncGetResponse (Bearer newToken)
                | _ ->
                    return raise (new Exception("Unexpected 403 error when using non-bearer token", ex))
    }