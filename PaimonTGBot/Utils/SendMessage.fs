module PaimonTGBot.Utils.SendMessage

open Funogram.Telegram
open Funogram.Telegram.Types

open PaimonTGBot.Utils.Bot

let private sendMessageFormatted (text: string) parseMode config chatId =
  Req.SendMessage.Make(ChatId.Int chatId, text, parseMode = parseMode) |> bot config

let sendMessageMarkdownV2 (text: string) config chatId =
  sendMessageFormatted text ParseMode.MarkdownV2 config chatId

let sendMessageHTML (text: string) config chatId =
  sendMessageFormatted text ParseMode.HTML config chatId

let sendMessagePlainText (text: string) config (chatId: int64) =
  Api.sendMessage chatId text |> bot config