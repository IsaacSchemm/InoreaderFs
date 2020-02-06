namespace InoreaderFs.Authentication

type BearerToken = {
    token: string
} with
    interface IInoreaderCredentials with
        member this.GetHeaders() = seq {
            ("Authorization", sprintf "Bearer %s" this.token)
        }