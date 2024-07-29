#r "nuget: OpenAI.Client"
open OpenAI
open OpenAI.Client
open OpenAI.Chat

let propmtPrefix propmt  = sprintf "请回答下面的的提示词，输出将会以TelegramBot的MarkdownV2模式输出，请考虑好转义字符等问题，避免出现问题，提示词为 \{ %s \}" propmt

let getChatResponse (config: Config) (message: string) =
    let message = propmtPrefix message
    match (config |> chat |> create { Model = "deepseek-chat"; Messages = [| {Role = "user"; Content = message} |]}) with
    | {Choices = [| {FinishReason = Some "content_filter"} |]} -> "触发了过滤规则喔，考虑一下重新措辞吧"
    | {Choices = [| {FinishReason = Some "length"} |]} -> "太长了喔，考虑下换个问法吧"
    | {Choices = [| {FinishReason = Some "insufficient_system_resource"} |]} -> "系统资源不足，稍后再试试吧"
    | {Choices = [| {FinishReason = Some "stop"; Message = completion} |]} -> completion.Content
    | _ -> "出现了未知错误，请稍后再试试吧"

let client =
    Config(
        { Endpoint = "https://api.deepseek.com";
                   ApiKey = "sk-c16e462dc1034ad1852124ac61c3dbeb"},
                 HttpRequester()
    )