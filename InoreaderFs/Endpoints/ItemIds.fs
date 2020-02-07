namespace InoreaderFs.Endpoints

open System
open System.IO
open FSharp.Json
open InoreaderFs

module ItemIds =
    type Order = OldestFirst = 1 | NewestFirst = 2

    type Request() =
        member val Number = 20 with get, set
        member val Order = Order.NewestFirst with get, set
        member val StartTime = Nullable<DateTimeOffset>() with get, set
        member val ExcludeRead = false with get, set
        member val IncludeRead = false with get, set
        member val IncludeStarred = false with get, set
        member val IncludeLike = false with get, set
        member val Continuation: string = null with get, set
        member val StreamId: string = null with get, set
        member val IncludeAllDirectStreamIds = true with get, set

        member this.GetParameters() = seq {
            ("n", sprintf "%d" this.Number)
            if this.Order = Order.OldestFirst then
                ("r", "o")
            if this.StartTime.HasValue then
                ("ot", sprintf "%d" (this.StartTime.Value.ToUnixTimeSeconds()))
            if this.ExcludeRead then
                ("xt", "user/-/state/com.google/read")
            if this.IncludeRead then
                ("it", "user/-/state/com.google/read")
            if this.IncludeStarred then
                ("it", "user/-/state/com.google/starred")
            if this.IncludeLike then
                ("it", "user/-/state/com.google/like")
            if not (isNull this.Continuation) then
                ("c", this.Continuation)
            if not (isNull this.StreamId) then
                ("s", this.StreamId)
            if not this.IncludeAllDirectStreamIds then
                ("includeAllDirectStreamIds", "false")
        }

    type ItemRef = {
        id: string
        directStreamIds: string list
        timestampUsec: string
    } with
        member this.GetTimestamp() =
            match Int64.TryParse this.timestampUsec with
            | (true, x) -> DateTimeOffset.FromUnixTimeMilliseconds 0L + new TimeSpan(x * 10L)
            | _ -> DateTimeOffset.MinValue

    type Response = {
        itemRefs: ItemRef list
        continuation: string option
    } with
        member this.GetContinuation() =
            Option.toObj this.continuation

    let AsyncExecute credentials (o: Request) = async {
        let qs = o.GetParameters() |> Shared.BuildForm
        let req = new InoreaderRequest(sprintf "/reader/api/0/stream/items/ids?%s" qs)
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