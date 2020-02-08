namespace InoreaderFs.Endpoints

open System
open System.IO
open FSharp.Json
open InoreaderFs

module SubscriptionList =
    type Category = {
        id: string
        label: string
    }

    type Subscription = {
        id: string
        title: string
        categories: Category list
        sortid: string
        firstitemmsec: int64
        url: string
        htmlUrl: string
        iconUrl: string
    } with
        member this.GetFirstItemTimestamp() =
            Shared.FromUnixTimeMicroseconds this.firstitemmsec

    type Response = {
        subscriptions: Subscription list
    }

    let AsyncExecute credentials = async {
        let req = new InoreaderRequest("/reader/api/0/subscription/list")

        use! resp = req.AsyncGetResponse credentials
        use respStream = resp.GetResponseStream()
        use sr = new StreamReader(respStream)

        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        return Json.deserialize<Response> json
    }

    let ExecuteAsync credentials =
        AsyncExecute credentials
        |> Async.StartAsTask