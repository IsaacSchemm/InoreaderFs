namespace InoreaderFs.Endpoints

open System.IO
open FSharp.Json
open InoreaderFs

module AddSubscription =
    type Response = {
        query: string
        numResults: int
        streamId: string
        streamName: string
    }

    let AsyncExecute credentials feedId = async {
        let req = new InoreaderRequest("/reader/api/0/subscription/quickadd")
        req.Method <- "POST"
        req.ContentType <- Some "application/x-www-form-urlencoded"
        req.Body <-
            seq {
                ("quickadd", feedId)
            }
            |> Shared.BuildForm
            |> System.Text.Encoding.UTF8.GetBytes
            |> Some

        use! resp = req.AsyncGetResponse credentials
        use respStream = resp.GetResponseStream()
        use sr = new StreamReader(respStream)

        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        return Json.deserialize<Response> json
    }

    let ExecuteAsync credentials feedId =
        AsyncExecute credentials feedId
        |> Async.StartAsTask