module Adacola.SoftOmeletRice.Main

open System
open System.IO
open System.Text
open Argu
open MathNet.Numerics

type Arguments =
    | Once
    | RetweetOnStart
with
    interface IArgParserTemplate with
        member x.Usage: string = 
            match x with
            | Once -> "起動時に一度だけリツイートしてすぐアプリを終了します"
            | RetweetOnStart -> "起動時にまずリツイートします。Onceが指定されている場合はOnceが優先されます"

let softOmeletRiceTweetID = TweetID 596307547985809408L

[<EntryPoint>]
let main argv =
    let argParser = ArgumentParser.Create<Arguments>(errorHandler = ProcessExiter())
    let parseResult = argParser.Parse argv
    let isOnce = parseResult.Contains <@ Once @>
    let retweetsOnStart = not isOnce && parseResult.Contains <@ RetweetOnStart @>

    let maybeConfig =
        let configFile = "config.json"
        if File.Exists configFile then
            File.ReadAllText(configFile, Encoding.UTF8) |> ConfigProvider.Parse |> Some
        else None
    let minHour = maybeConfig |> Option.bind (fun x -> x.MinHour) |> defaultArg <| 72 |> max 1 |> min 480
    let maxHour = maybeConfig |> Option.bind (fun x -> x.MaxHour) |> defaultArg <| 168 |> max (minHour + 1) |> min 481
    let consumerKey = File.ReadAllText("consumerKey.json", Encoding.UTF8) |> ConsumerKeyProvider.Parse
    let accessToken = File.ReadAllText("accessToken.json", Encoding.UTF8) |> AccessTokenProvider.Parse
    let token = Twitter.authenticate consumerKey accessToken
    let retweet() =
        async {
            let! response = Twitter.unretweetAndRetweet token softOmeletRiceTweetID
            do Console.WriteLine("{0} にリツイートしました。ID : {1}", DateTime.Now, response.Id)
        }

    if isOnce || retweetsOnStart then retweet() |> Async.RunSynchronously
    if not isOnce then
        Console.WriteLine("Enterを押すと終了します")
        async {
            try
                let random = Random.MersenneTwister()
                while true do
                    let waitHour = random.Next(minHour, maxHour)
                    let waitSecond = random.Next(3600)
                    let waitTotalSecond = waitHour * 3600 + waitSecond
                    let waitTimeSpan = waitTotalSecond |> float |> TimeSpan.FromSeconds
                    let waitEndTime = DateTime.Now + waitTimeSpan
                    do Console.WriteLine("{0} スリープします。 {1} にリツイート", waitTimeSpan, waitEndTime)
                    let waitTotalMilliSecond = waitTotalSecond * 1000
                    do! Async.Sleep waitTotalMilliSecond
                    do! retweet()
            with
            | e -> do Console.Error.WriteLine e
        } |> Async.Start
        Console.ReadLine() |> ignore
    0
