namespace InoreaderFs.Auth.OAuth

open InoreaderFs.Auth
open System.Threading.Tasks

/// An object that contains access and refresh tokens for Inoreader and
/// gives InoreaderFs the ability to update those tokens automatically.
type IAutoRefreshToken =
    inherit IRefreshToken

    /// The app ID and key.
    abstract member App: App with get

    /// Replaces the access and refresh tokens with new values in the
    /// backing store.
    abstract member UpdateTokenAsync: IRefreshToken -> Task