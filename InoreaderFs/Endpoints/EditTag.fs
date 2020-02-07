namespace InoreaderFs.Endpoints

open InoreaderFs

module EditTag =
    type Request() =
        member val TagToAdd: string = null with get, set
        member val TagToRemove: string = null with get, set
        member val ItemID: string seq = Seq.empty with get, set

        member this.GetParameters() = seq {
            if not (isNull this.TagToAdd) then
                ("a", this.TagToAdd)
            if not (isNull this.TagToRemove) then
                ("r", this.TagToRemove)
            for item in this.ItemID do
                ("i", item)
        }

    let AsyncExecute credentials (o: Request) = async {
        let req = new InoreaderRequest("/reader/api/0/edit-tag")
        req.Method <- "POST"
        req.ContentType <- Some "application/x-www-form-urlencoded"
        let m =
            o.GetParameters()
            |> Shared.BuildForm
        req.Body <-
            m
            |> System.Text.Encoding.UTF8.GetBytes
            |> Some

        use! resp = req.AsyncGetResponse credentials
        ignore resp
    }

    let ExecuteAsync credentials o =
        AsyncExecute credentials o
        |> Async.StartAsTask