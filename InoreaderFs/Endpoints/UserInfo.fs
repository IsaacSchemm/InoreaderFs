namespace InoreaderFs.Endpoints

open System
open System.Net
open System.IO
open FSharp.Json
open InoreaderFs

module UserInfo =
    type Response = {
        userId: string
        userName: string
        userProfileId: string
        userEmail: string
        isBloggerUser: bool
        [<JsonField(Transform=typeof<Transforms.DateTimeOffsetEpoch>)>]
        signupTimeSec: DateTimeOffset
        isMultiLoginEnabled: bool
    }

    let AsyncExecute (credentials: Credentials) = async {
        let req = WebRequest.CreateHttp "https://www.inoreader.com/reader/api/0/user-info"
        for (k, v) in credentials.Headers do
            req.Headers.[k] <- v

        use! resp = req.AsyncGetResponse()
        use respStream = resp.GetResponseStream()
        use sr = new StreamReader(respStream)

        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        return Json.deserialize<Response> json
    }

    let ExecuteAsync credentials =
        AsyncExecute credentials
        |> Async.StartAsTask