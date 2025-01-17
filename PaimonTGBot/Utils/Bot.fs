﻿module PaimonTGBot.Utils.Bot

open Funogram.Types
open Funogram.Api

let processResultWithValue (result: Result<'a, ApiResponseError>) =
  match result with
  | Ok v -> Some v
  | Error e ->
    printfn "Server error: %s" e.Description
    None

let processResult (result: Result<'a, ApiResponseError>) =
  processResultWithValue result |> ignore

let botResult config data = api config data |> Async.RunSynchronously
let bot config data = botResult config data |> processResult