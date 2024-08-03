module PaimonTGBot.Commands.Aichat

open System
open System.IO
open System.Threading.Tasks

open OpenAI
open OpenAI.Client
open OpenAI.Chat

[<Literal>]
let TokenFileName = "ai-token"

let propmtPrefix propmt  = sprintf "%s" propmt

let defaultAichatConfig =
        { Endpoint = "https://api.deepseek.com";
                   ApiKey = ""}

let withReadTokenFromFile config =
    if File.Exists(TokenFileName) then
        { config with ApiKey = File.ReadAllText(TokenFileName).Replace("\n", "").Replace("\r", "") }
    else
        printf "Please, enter ai token: "
        let token = Console.ReadLine()
        File.WriteAllText(TokenFileName, token)
        { config with ApiKey = token }

let getChatResponseAsync (config: ApiConfig) (message: string) =
    async {
        let client = Config(config, HttpRequester())
        let message = propmtPrefix message
        let! response = 
            Task.Run( fun () ->
                client 
                |> chat 
                |> create { Model = "deepseek-chat"; Messages = [| {Role = "user"; Content = message} |] }
            )
            |> Async.AwaitTask
            
        match response with
        | {Choices = [| {FinishReason = Some "content_filter"} |]} -> return "触发了过滤规则喔，考虑一下重新措辞吧"
        | {Choices = [| {FinishReason = Some "length"} |]} -> return "太长了喔，考虑下换个问法吧"
        | {Choices = [| {FinishReason = Some "insufficient_system_resource"} |]} -> return "系统资源不足，稍后再试试吧"
        | {Choices = [| {FinishReason = Some "stop"; Message = completion} |]} -> return completion.Content
        | _ -> return "出现了未知错误，请稍后再试试吧"
    }