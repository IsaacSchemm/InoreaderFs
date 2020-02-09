namespace InoreaderFs

open System
open System.Net
open InoreaderFs.Auth
open InoreaderFs.Auth.OAuth
open System.IO

type InoreaderRequest(path: string) =
    let InoreaderUri = new Uri("https://www.inoreader.com/")
    let UserAgent = Shared.UserAgent

    let Is403 (response: WebResponse) =
        match response with
        | :? HttpWebResponse as r -> r.StatusCode = HttpStatusCode.Forbidden
        | _ -> false

    member val Method = "GET" with get, set
    member val ContentType: string option = None with get, set
    member val Body: byte[] option = None with get, set

    member this.AsyncGetResponse (credentials: Credentials) = async {
        let req = new Uri(InoreaderUri, path) |> WebRequest.CreateHttp
        req.Method <- this.Method
        req.UserAgent <- UserAgent

        match this.ContentType with
        | Some s -> req.ContentType <- s
        | None -> ()

        for (k, v) in credentials.Headers do
            req.Headers.[k] <- v

        match this.Body with
        | Some b ->
            do! async {
                use! reqStream = req.GetRequestStreamAsync() |> Async.AwaitTask
                use ms = new MemoryStream(b, false)
                do! ms.CopyToAsync reqStream |> Async.AwaitTask
            }
        | None -> ()

        match credentials with
        | OAuth (:? IAutoRefreshToken as auto) ->
            try
                return! req.AsyncGetResponse()
            with
                | :? WebException as ex when Is403 ex.Response ->
                    do! TokenTools.AsyncRefresh auto
                    let newToken = TokenTools.NoRefresh auto
                    return! this.AsyncGetResponse (OAuth newToken)
        | _ ->
            return! req.AsyncGetResponse()
    }

    member this.GetResponseAsync credentials =
        this.AsyncGetResponse credentials
        |> Async.StartAsTask