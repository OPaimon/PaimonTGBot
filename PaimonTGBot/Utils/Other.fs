module PaimonTGBot.Utils.Other

open System.Text

let containsNonAsciiChars (input: string) : bool =
    try
        let asciiBytes = Encoding.ASCII.GetBytes(input)
        let roundTrip = Encoding.ASCII.GetString(asciiBytes)
        input = roundTrip
    with :? System.ArgumentException ->
        false
