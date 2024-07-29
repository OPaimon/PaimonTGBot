module PaimonTGBot.GConfig
open OpenAI

type Config = {
    openAIApiConfig: OpenAI.ApiConfig
}