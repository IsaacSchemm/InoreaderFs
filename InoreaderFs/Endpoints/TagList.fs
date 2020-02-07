namespace InoreaderFs.Endpoints

open System.IO
open FSharp.Json
open InoreaderFs

module TagList =
    type Request() =
        member val Types = false with get, set
        member val Counts = false with get, set

        member this.GetParameters() = seq {
            if this.Types then
                ("types", "1")
            if this.Counts then
                ("counts", "1")
        }

    type Tag = {
        id: string
        sortid: string
        ``type``: string option
        unread_count: int option
        unseen_count: int option
    } with
        member this.GetType() = Option.toObj this.``type``
        member this.GetUnreadCount() = Option.toNullable this.unread_count
        member this.GetUnseenCount() = Option.toNullable this.unseen_count

    type Response = {
        tags: Tag list
    }

    let AsyncExecute credentials (o: Request) = async {
        let qs = o.GetParameters() |> Shared.BuildForm
        let req = new InoreaderRequest(sprintf "/reader/api/0/tag/list?%s" qs)
        req.ContentType <- Some "application/x-www-form-urlencoded"

        use! resp = req.AsyncGetResponse credentials
        use respStream = resp.GetResponseStream()
        use sr = new StreamReader(respStream)

        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        return Json.deserialize<Response> json
    }

    let ExecuteAsync credentials o =
        AsyncExecute credentials o
        |> Async.StartAsTask