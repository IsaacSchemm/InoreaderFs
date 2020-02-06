namespace InoreaderFs.Endpoints

open InoreaderFs

module EditSubscription =
    type Request() =
        member val Action = "" with get, set
        member val StreamId = "" with get, set
        member val SubscriptionTitle: string = null with get, set
        member val AddToFolder: string = null with get, set
        member val RemoveFromFolder: string = null with get, set

        member this.GetParameters() = seq {
            ("ac", this.Action)
            ("s", this.StreamId)
            if not (isNull this.SubscriptionTitle) then
                ("t", this.SubscriptionTitle)
            if not (isNull this.AddToFolder) then
                ("a", this.AddToFolder)
            if not (isNull this.RemoveFromFolder) then
                ("r", this.RemoveFromFolder)
        }

    let AsyncExecute credentials (o: Request) = async {
        let req = new InoreaderRequest("/reader/api/0/subscription/edit")
        req.Method <- "POST"
        req.ContentType <- Some "application/x-www-form-urlencoded"
        req.Body <-
            o.GetParameters()
            |> QueryStringBuilder.BuildForm
            |> System.Text.Encoding.UTF8.GetBytes
            |> Some

        use! resp = req.AsyncGetResponse credentials
        ignore resp
    }

    let ExecuteAsync credentials feedId =
        AsyncExecute credentials feedId
        |> Async.StartAsTask