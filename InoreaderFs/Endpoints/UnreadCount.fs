namespace InoreaderFs.Endpoints

open System
open System.IO
open FSharp.Json
open InoreaderFs

module UnreadCount =
    type Count = {
        id: string
        count: obj
        newestItemTimestampUsec: string
    } with
        member this.CountAsInt32 =
            this.count
            |> sprintf "%O"
            |> Int32.Parse
        member this.NewestItemTimestamp =
            match Int64.TryParse this.newestItemTimestampUsec with
            | (true, x) -> DateTimeOffset.FromUnixTimeMilliseconds(0L) + new TimeSpan(x * 10L)
            | _ -> DateTimeOffset.MinValue

    type Response = {
        max: string
        unreadcounts: Count list
    }

    let AsyncExecute credentials = async {
        let req = new InoreaderRequest("/reader/api/0/unread-count")

        use! resp = req.AsyncGetResponse credentials
        use respStream = resp.GetResponseStream()
        use sr = new StreamReader(respStream)

        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        return Json.deserializeEx<Response> (JsonConfig.create(allowUntyped = true)) json
    }

    let ExecuteAsync credentials =
        AsyncExecute credentials
        |> Async.StartAsTask