namespace InoreaderFs.Auth

open System.Threading

module TokenTools =
    let private RefreshLock = new SemaphoreSlim(1, 1)

    let AsyncRefresh (t: IAutoRefreshToken) = async {
        do! RefreshLock.WaitAsync() |> Async.AwaitTask
        try
            let oAuth = new OAuth(t.App)
            let! newToken = oAuth.AsyncRefresh t.RefreshToken
            do! t.UpdateTokenAsync newToken |> Async.AwaitTask
        finally
            RefreshLock.Release() |> ignore
    }

    let RefreshAsync t =
        AsyncRefresh t
        |> Async.StartAsTask

    let NoRefresh (t: IRefreshToken) = {
        new IBearerToken with
            member __.AccessToken = t.AccessToken
    }