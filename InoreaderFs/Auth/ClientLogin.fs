namespace InoreaderFs.Auth

open System.Net
open System.IO
open System

type ClientLoginException(message: string, body: string) =
    inherit Exception(message)

    member __.Body = body

type ClientLoginAuth = ClientLoginAuth of string

module ClientLogin =
    let AsyncLogin email password = async {
        let req = WebRequest.CreateHttp "https://www.inoreader.com/accounts/ClientLogin"
        req.Method <- "POST"
        req.ContentType <- "application/x-www-form-urlencoded"

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

    let LoginAsync email password =
        AsyncLogin email password
        |> Async.StartAsTask