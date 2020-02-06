namespace InoreaderFs.Authentication

type IInoreaderCredentials =
    abstract member GetHeaders: unit -> seq<string * string>