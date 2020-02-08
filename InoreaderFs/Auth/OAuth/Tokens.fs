namespace InoreaderFs.Auth.OAuth

/// An object that has an access token for the Inoreader API.
type IBearerToken =
    abstract member AccessToken: string

/// An object that has access and refresh tokens for the Inoreader API.
type IRefreshToken =
    inherit IBearerToken
    abstract member RefreshToken: string

/// An Inoreader API token returned from the OAuth2 "token" endpoint.
type RefreshToken = {
  access_token: string
  token_type: string
  expires_in: int
  refresh_token: string
  scope: string
} with
    interface IRefreshToken with
        member this.AccessToken = this.access_token
        member this.RefreshToken = this.refresh_token