namespace InoreaderFs.Endpoints

open InoreaderFs

module StreamPreferencesSet =
    type Request = {
        StreamId: string
        Key: string
        Value: string
    } with
        member this.GetParameters() = seq {
            ("s", this.StreamId)
            ("k", this.Key)
            ("v", this.Value)
        }

    let AsyncExecute credentials (o: Request) = async {
        let req = new InoreaderRequest("/reader/api/0/preference/stream/set")
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