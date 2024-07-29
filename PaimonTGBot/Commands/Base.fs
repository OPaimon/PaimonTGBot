module PaimonTGBot.Commands.Base

open System
open System.Text
open System.Text.RegularExpressions
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.Telegram.Bot
open PaimonTGBot
open PaimonTGBot.Utils.Markdown
open PaimonTGBot.Utils.SendMessage
open PaimonTGBot.Commands.CmdEcho
open PaimonTGBot.Commands.Aichat

let updateArrived (config: PaimonTGBot.GConfig.Config) (ctx: UpdateContext) =
    let wrap fn =
        let fromId () = ctx.Update.Message.Value.From.Value.Id
        fn ctx.Config (fromId ())
    match ctx with
    | { Update = { Message = Some { Text = Some text } } } when text.StartsWith "/chat " ->
        text.Substring(5) |> Aichat.getChatResponse config.openAIApiConfig |> sendMessagePlainText |> wrap
    | _ -> ()

    cmdEcho ctx
