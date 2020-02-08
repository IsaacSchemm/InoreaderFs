open System
open InoreaderFs.Auth
open InoreaderFs.Auth.ClientLogin

[<EntryPoint>]
let main _ = Async.RunSynchronously (async {
    printf "App ID: "
    let appId = Console.ReadLine()
    printf "App key: "
    let appKey = Console.ReadLine()
    let app = { appId = appId; appKey = appKey }

    printf "Email: "
    let email = Console.ReadLine()
    printf "Password: "
    let password = Console.ReadLine()
    let! auth = ClientLoginHandler.AsyncLogin email password

    let credentials = ClientLogin (app, auth)

    printfn "%A" credentials

    let! user = InoreaderFs.Endpoints.UserInfo.AsyncExecute credentials
    printfn "%A" user

    return 0
})