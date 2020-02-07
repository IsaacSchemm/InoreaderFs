namespace InoreaderFs.Endpoints

open System.IO
open FSharp.Json
open InoreaderFs

module StreamPreferencesList =
    type StreamPrefs = {
        id: string
        value: string
    }

    type Response = {
        streamprefs: Map<string, StreamPrefs list>
    }

    let AsyncExecute credentials = async {
        let req = new InoreaderRequest("/reader/api/0/preference/stream/list")

        use! resp = req.AsyncGetResponse credentials
        use respStream = resp.GetResponseStream()
        use sr = new StreamReader(respStream)

        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        return Json.deserialize<Response> json
    }

    let ExecuteAsync credentials =
        AsyncExecute credentials
        |> Async.StartAsTask