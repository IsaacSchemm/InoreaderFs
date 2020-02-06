namespace InoreaderFs.Auth

open System.Net
open System.IO
open System
open InoreaderFs

/// An exception thrown when the ClientLogin module tries and fails to handle an HTTP error.
type ClientLoginException(message: string, body: string) =
    inherit Exception(message)

    member __.Body = body

/// An object that stores an Auth token from the deprecated ClientLogin flow.
type ClientLoginAuth = ClientLoginAuth of string

/// Implements the deprecated ClientLogin flow of the Inoreader API.
module ClientLogin =
    /// Acquires an Auth token from the server.
    let AsyncLogin email password = async {
        let req = WebRequest.CreateHttp "https://www.inoreader.com/accounts/ClientLogin"
        req.Method <- "POST"
        req.ContentType <- "application/x-www-form-urlencoded"
        req.UserAgent <- Shared.UserAgent

        do! async {
            use! reqStream = req.GetRequestStreamAsync() |> Async.AwaitTask
            use sw = new StreamWriter(reqStream)
            do! sprintf "Email=%s&Passwd=%s" (Uri.EscapeDataString email) (Uri.EscapeDataString password)
                |> sw.WriteAsync
                |> Async.AwaitTask
        }

        use! resp = req.AsyncGetResponse()
        use respStream = resp.GetResponseStream()
        use sr = new StreamReader(respStream)

        let! body = sr.ReadToEndAsync() |> Async.AwaitTask
        let fields = seq {
            use tr = new StringReader(body)
            let mutable ended = false
            while not ended do
                let line = tr.ReadLine()
                if isNull line then
                    ended <- true
                else
                    let split = line.Split '='
                    if split.Length <> 2 then
                        raise (new ClientLoginException("Malformed response from ClientLogin", body))
                    yield (split.[0], split.[1])
        }

        let auth =
            fields
            |> Seq.where (fun p -> fst p = "Auth")
            |> Seq.map snd
            |> Seq.tryHead
        return
            match auth with
            | Some v -> ClientLoginAuth v
            | None -> raise (new ClientLoginException("No Auth= in response", body))
    }

    /// Acquires an Auth token from the server.
    let LoginAsync email password =
        AsyncLogin email password
        |> Async.StartAsTask