namespace InoreaderFs

open System

module internal Shared =
    let UserAgent = "InoreaderFs/0.0 (https://github.com/IsaacSchemm/InoreaderFs)"

    let BuildForm (dict: seq<string * string>) =
        let parameters = seq {
            for k, v in dict do
                if isNull v then
                    failwithf "Null values in form not allowed"
                let key = Uri.EscapeDataString k
                let value = Uri.EscapeDataString v
                yield sprintf "%s=%s" key value
        }
        String.concat "&" parameters

    let ToUnixTimeMicroseconds (ts: DateTimeOffset) =
        (ts - DateTimeOffset.FromUnixTimeMilliseconds 0L).Ticks / 10L