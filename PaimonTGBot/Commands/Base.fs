module PaimonTGBot.Commands.Base

open System
open System.Text
open System.Text.RegularExpressions
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.Telegram.Bot

let escapeMarkdown (text: string) : string =
    let escapeChars = @"\_*[]()~`>#+-=|{}.!"

    let escapedChars = Regex.Escape(escapeChars)
    let pattern = sprintf @"([%s])" escapedChars
    Regex.Replace(text, pattern, @"\\\1")

let mentionMarkdownV2 (username: string, userid: int64) =
    sprintf "[%s](tg://user?id=%d)" username userid

let containsNonAsciiChars (input: string) : bool =
    try
        let asciiBytes = Encoding.ASCII.GetBytes(input)
        let roundTrip = Encoding.ASCII.GetString(asciiBytes)
        input = roundTrip
    with :? System.ArgumentException ->
        false

[<Literal>]
let testMDV2 = ""

let updateArrived (ctx: UpdateContext) =
    let mentionUser firstName lastName userId =
        match lastName with
        | Some ln -> mentionMarkdownV2 (sprintf "%s %s" firstName ln, userId)
        | None -> mentionMarkdownV2 (firstName, userId)
    match ctx.Update.Message with
    | Some { MessageId = messageId
             Chat = chat
             Text = Some text
             From = Some { Id = userId
                           FirstName = firstName
                           LastName = lastName }
             ReplyToMessage = replyToMessage } when text.StartsWith "/" || text.StartsWith "\\" ->
        let isPassive = text.StartsWith("/") |> not
        let commandTexts = text.Substring(1).Split(" ", 2, StringSplitOptions.None)

        match containsNonAsciiChars commandTexts.[0] with
        | true -> ()
        | false ->
            let FromMention = mentionUser firstName lastName userId
            let ToMention =  match replyToMessage with
                                 | Some { From = Some { Id = toUserId
                                                        FirstName = toFirstName
                                                        LastName = toLastName } } -> mentionUser toFirstName toLastName toUserId
                                 | _ -> "自己"

            let selfActOnRepliedUser verb =
                sprintf "%s %s了 %s！" FromMention verb ToMention

            let selfAct1RepliedUserAct2 (verbs: string * string) =
                let cmd1, cmd2 = verbs
                sprintf "%s %s %s %s！" FromMention cmd1 ToMention cmd2

            let selfActedOnByRepliedUser verb =
                sprintf "%s 被 %s %s 了！" FromMention ToMention verb

            let repliedUserAct1SelfAct2 (verbs: string * string) =
                let cmd1, cmd2 = verbs
                sprintf "%s %s %s %s！" ToMention cmd1 FromMention cmd2

            let resultMessage =
                match commandTexts.Length with
                | 1 ->
                    match isPassive with
                    | true -> commandTexts.[0] |> escapeMarkdown |> selfActedOnByRepliedUser
                    | false -> commandTexts.[0] |> escapeMarkdown |> selfActOnRepliedUser
                | 2 ->
                    let verbs = (escapeMarkdown commandTexts.[0]), (escapeMarkdown commandTexts.[1])
                    match isPassive with
                    | true -> verbs |> repliedUserAct1SelfAct2
                    | false -> verbs |> selfAct1RepliedUserAct2
                | _ -> ""

            Req.SendMessage.Make(ChatId.Int chat.Id, resultMessage, parseMode = ParseMode.MarkdownV2)
            |> api ctx.Config
            |> Async.Ignore
            |> Async.Start
    | _ -> ()
