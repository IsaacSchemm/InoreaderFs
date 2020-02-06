namespace InoreaderFs.Endpoints

open System
open System.IO
open FSharp.Json
open InoreaderFs
open InoreaderFs.Auth

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

    let AsyncExecute credentials = async {
        let req = new InoreaderRequest("/reader/api/0/user-info")

        use! resp = req.AsyncGetResponse credentials
        use respStream = resp.GetResponseStream()
        use sr = new StreamReader(respStream)

        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        return Json.deserialize<Response> json
    }

    let ExecuteAsync credentials =
        AsyncExecute credentials
        |> Async.StartAsTask