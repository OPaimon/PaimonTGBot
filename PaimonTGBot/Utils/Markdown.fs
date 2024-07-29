module PaimonTGBot.Utils.Markdown

open System.Text.RegularExpressions

let escapeMarkdown (text: string) : string =
    let escapedChars = @"\\_\*\[\]\(\)~`>\#\+-=\|\{}\.!"
    let pattern = sprintf @"([%s])" escapedChars
    Regex.Replace(text, pattern, @"\$1")

let mentionMarkdownV2 (username: string, userid: int64) =
    sprintf "[%s](tg://user?id=%d)" username userid
