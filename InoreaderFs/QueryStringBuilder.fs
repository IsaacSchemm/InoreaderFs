namespace InoreaderFs

open System

module internal QueryStringBuilder =
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