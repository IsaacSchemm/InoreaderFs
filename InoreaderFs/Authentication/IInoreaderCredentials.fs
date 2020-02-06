namespace InoreaderFs.Authentication

open System.Collections.Generic

type IInoreaderCredentials =
    abstract member GetHeaders: unit -> IDictionary<string, string>