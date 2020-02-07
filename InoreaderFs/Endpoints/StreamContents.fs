namespace InoreaderFs.Endpoints

open System
open System.IO
open FSharp.Json
open InoreaderFs

module StreamContents =
    type Order = OldestFirst = 1 | NewestFirst = 2

    type RequestOptions() =
        member val Number = 20 with get, set
        member val Order = Order.NewestFirst with get, set
        member val StartTime = Nullable<DateTimeOffset>() with get, set
        member val ExcludeRead = false with get, set
        member val IncludeStarred = false with get, set
        member val IncludeLike = false with get, set
        member val Continuation: string = null with get, set
        member val SearchQuery: string = null with get, set
        member val GlobalSearch = false with get, set
        member val IncludeAllDirectStreamIds = true with get, set

        member this.GetParameters() = seq {
            ("n", sprintf "%d" this.Number)
            if this.Order = Order.OldestFirst then
                ("r", "o")
            if this.StartTime.HasValue then
                ("ot", sprintf "%d" (this.StartTime.Value.ToUnixTimeSeconds()))
            if this.ExcludeRead then
                ("xt", "user/-/state/com.google/read")
            if this.IncludeStarred then
                ("it", "user/-/state/com.google/starred")
            if this.IncludeLike then
                ("it", "user/-/state/com.google/like")
            if not (isNull this.Continuation) then
                ("c", this.Continuation)
            if not (isNull this.SearchQuery) then
                ("sq", this.SearchQuery)
                if this.GlobalSearch then
                    ("globalSearch", "1")
            if not this.IncludeAllDirectStreamIds then
                ("includeAllDirectStreamIds", "false")
        }

    type Canonical = {
        href: string
    }

    type Alternate = {
        href: string
        ``type``: string
    }

    type Summary = {
        direction: string
        content: string
    }

    type Origin = {
        streamId: string
        title: string
        htmlUrl: string
    }

    type Item = {
        crawlTimeMsec: string
        timestampUsec: string
        id: string
        categories: string list
        title: string
        published: int64
        updated: int64
        canonical: Canonical list
        alternate: Alternate list
        summary: Summary
        author: string
        commentsNum: int
        origin: Origin
    } with
        member this.GetTimestamp() =
            match Int64.TryParse this.timestampUsec with
            | (true, x) -> DateTimeOffset.FromUnixTimeMilliseconds 0L + new TimeSpan(x * 10L)
            | _ -> DateTimeOffset.MinValue
        member this.GetPublished() =
            DateTimeOffset.FromUnixTimeSeconds this.published
        member this.GetUpdated() =
            match this.updated with
            | 0L -> Nullable()
            | x -> Nullable (DateTimeOffset.FromUnixTimeSeconds x)

    type Response = {
        direction: string
        id: string
        title: string
        description: string
        self: Canonical
        updated: int64
        updatedUsec: string
        items: Item list
        continuation: string option
    } with
        member this.GetUpdated() =
            match Int64.TryParse this.updatedUsec with
            | (true, x) -> DateTimeOffset.FromUnixTimeMilliseconds 0L + new TimeSpan(x * 10L)
            | _ -> DateTimeOffset.MinValue
        member this.GetContinuation() =
            Option.toObj this.continuation

    let AsyncExecute credentials streamId (o: RequestOptions) = async {
        let qs = o.GetParameters() |> Shared.BuildForm
        let req = new InoreaderRequest(sprintf "/reader/api/0/stream/contents/%s?%s" (Uri.EscapeDataString streamId) qs)
        req.ContentType <- Some "application/x-www-form-urlencoded"

        use! resp = req.AsyncGetResponse credentials
        use respStream = resp.GetResponseStream()
        use sr = new StreamReader(respStream)

        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        return Json.deserialize<Response> json
    }

    let ExecuteAsync credentials streamId o =
        AsyncExecute credentials streamId o
        |> Async.StartAsTask