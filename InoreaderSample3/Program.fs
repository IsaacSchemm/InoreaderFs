open System
open InoreaderFs.Auth
open InoreaderFs.Auth.ClientLogin
open InoreaderFs.Auth.OAuth

let get_credentials () = async {
    printfn "(1) OAuth access token"
    printfn "(2) ClientLogin"
    let opt = Console.ReadLine()

    match opt with
    | "1" ->
        printf "Access token: "
        let accessToken = Console.ReadLine()
        let tokenObj = {
            new IAccessToken with
                member __.AccessToken = accessToken
        }
        return OAuth tokenObj
    | "2" ->
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

        return ClientLogin (app, auth)
    | _ ->
        return failwith "Invalid number entered"
}

[<EntryPoint>]
let main _ = Async.RunSynchronously (async {
    let! credentials = get_credentials ()
    printfn "%A" credentials

    let! user = InoreaderFs.Endpoints.UserInfo.AsyncExecute credentials
    printfn "%A" user

    return 0
})