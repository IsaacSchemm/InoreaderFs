namespace InoreaderFs.Auth

open System.Threading.Tasks

type IAutoRefreshToken =
    inherit IRefreshToken
    abstract member App: App with get
    abstract member UpdateTokenAsync: IRefreshToken -> Task