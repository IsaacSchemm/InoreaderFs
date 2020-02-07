namespace InoreaderFs.Endpoints

open InoreaderFs

module RenameTag =
    type Request = {
        Source: string
        Target: string
    } with
        member this.GetParameters() = seq {
            ("s", this.Source)
            ("dest", this.Target)
        }

    let AsyncExecute credentials (o: Request) = async {
        let req = new InoreaderRequest("/reader/api/0/rename-tag")
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