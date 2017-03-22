module Adacola.SoftOmeletRice.Main

open System
open System.IO
open System.Text

[<EntryPoint>]
let main argv =
    let config = File.ReadAllText("config.json", Encoding.UTF8) |> ConfigProvider.Parse
    let consumerKey = File.ReadAllText("consumerKey.json", Encoding.UTF8) |> ConsumerKeyProvider.Parse
    let accessToken = File.ReadAllText("accessToken.json", Encoding.UTF8) |> AccessTokenProvider.Parse
    let retweet = Retweet.configToRetweet consumerKey accessToken config

    Console.WriteLine("Enterを押すと終了します")
    retweet |> Async.Start
    Console.ReadLine() |> ignore
    0
