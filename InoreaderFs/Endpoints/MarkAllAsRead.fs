namespace InoreaderFs.Endpoints

open System
open InoreaderFs

module MarkAllAsRead =
    type Request = {
        Timestamp: DateTimeOffset
        StreamId: string
    } with
        member this.GetParameters() = seq {
            ("ts", sprintf "%d" (Shared.ToUnixTimeMicroseconds this.Timestamp))
            ("s", this.StreamId)    
        }

    let AsyncExecute credentials (o: Request) = async {
        let req = new InoreaderRequest("/reader/api/0/mark-all-as-read")
        req.Method <- "POST"
        req.ContentType <- Some "application/x-www-form-urlencoded"
        req.Body <-
            o.GetParameters()
            |> Shared.BuildForm
            |> System.Text.Encoding.UTF8.GetBytes
            |> Some

        use! resp = req.AsyncGetResponse credentials
        ignore resp
    }

    let ExecuteAsync credentials o =
        AsyncExecute credentials o
        |> Async.StartAsTask