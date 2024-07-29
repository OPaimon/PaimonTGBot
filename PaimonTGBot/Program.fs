module PaimonTGBot.Program

open System
open PaimonTGBot
open PaimonTGBot.Commands
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot
  
type ConsoleLogger(color: ConsoleColor) =
  interface Funogram.Types.IBotLogger with
    member x.Log(text) =
      let fc = Console.ForegroundColor
      Console.ForegroundColor <- color
      Console.WriteLine(text)
      Console.ForegroundColor <- fc
    member x.Enabled = true

  
[<EntryPoint>]
let main _ =
  async {
    let aiConfig = Aichat.defaultAichatConfig |> Aichat.withReadTokenFromFile
    let config: PaimonTGBot.GConfig.Config = {openAIApiConfig = aiConfig}
    let botConfig = Config.defaultConfig |> Config.withReadTokenFromFile
    let botConfig =
      { botConfig with
          RequestLogger = Some (ConsoleLogger(ConsoleColor.Green)) }
    let! _ = Api.deleteWebhookBase () |> api botConfig
    return! startBot botConfig (Commands.Base.updateArrived config) None
  } |> Async.RunSynchronously
  0