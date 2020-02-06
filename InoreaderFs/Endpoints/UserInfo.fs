namespace InoreaderFs.Endpoints

open InoreaderFs.Authentication
open System.Net
open System.IO

module UserInfo =
    let AsyncExecute (credentials: IInoreaderCredentials) = async {
        let req = WebRequest.CreateHttp "https://www.inoreader.com/reader/api/0/user-info"
        for p in credentials.GetHeaders() do
            req.Headers.[p.Key] <- p.Value

        use! resp = req.AsyncGetResponse()
        use respStream = resp.GetResponseStream()
        use sr = new StreamReader(respStream)

        return! sr.ReadToEndAsync() |> Async.AwaitTask
    }

    let ExecuteAsync credentials =
        AsyncExecute credentials
        |> Async.StartAsTask