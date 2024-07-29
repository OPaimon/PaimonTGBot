#r "nuget: OpenAI.Client"
open OpenAI
open OpenAI.Client
open OpenAI.Chat

let propmtPrefix propmt  = sprintf "��ش�����ĵ���ʾ�ʣ����������TelegramBot��MarkdownV2ģʽ������뿼�Ǻ�ת���ַ������⣬����������⣬��ʾ��Ϊ \{ %s \}" propmt

let getChatResponse (config: Config) (message: string) =
    let message = propmtPrefix message
    match (config |> chat |> create { Model = "deepseek-chat"; Messages = [| {Role = "user"; Content = message} |]}) with
    | {Choices = [| {FinishReason = Some "content_filter"} |]} -> "�����˹��˹���ร�����һ�����´�ǰ�"
    | {Choices = [| {FinishReason = Some "length"} |]} -> "̫����ร������»����ʷ���"
    | {Choices = [| {FinishReason = Some "insufficient_system_resource"} |]} -> "ϵͳ��Դ���㣬�Ժ������԰�"
    | {Choices = [| {FinishReason = Some "stop"; Message = completion} |]} -> completion.Content
    | _ -> "������δ֪�������Ժ������԰�"

let client =
    Config(
        { Endpoint = "https://api.deepseek.com";
                   ApiKey = "sk-c16e462dc1034ad1852124ac61c3dbeb"},
                 HttpRequester()
    )