namespace InoreaderFs.Auth

/// The app ID and app key of a registered Inoreader application. Used in both OAuth and ClientLogin authentication.
type App = {
    appId: string
    appKey: string
}