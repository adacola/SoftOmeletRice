module Adacola.SoftOmeletRice.Retweet

open System

let getRetweet (random : Random) (token : CoreTweet.Tokens) (retweet : ConfigProvider.Retweet) =
    let tweetId = TweetID retweet.TweetId
    let name = retweet.Name |> defaultArg <| string retweet.TweetId
    let minHour = retweet.MinHour |> defaultArg <| 72 |> max 1 |> min 480
    let maxHour = retweet.MaxHour |> defaultArg <| 168 |> max (minHour + 1) |> min 481
    let maybeExcludeStartEnd = retweet.ExcludeHour |> Option.map (fun x -> x.StartHour, x.EndHour)

    async {
        try
            while true do
                let waitHour = random.Next(minHour, maxHour)
                let waitSecond = random.Next(3600)
                let waitTotalSecond = waitHour * 3600 + waitSecond
                let waitTimeSpan = waitTotalSecond |> float |> TimeSpan.FromSeconds
                let waitEndTime = DateTime.Now + waitTimeSpan
                match maybeExcludeStartEnd with
                | Some(s, e) when s <= waitEndTime.Hour && waitEndTime.Hour < e -> do ()
                | _ -> 
                    do Console.WriteLine("{0} : {1} スリープします。 {2} にリツイート", name, waitTimeSpan, waitEndTime)
                    let waitTotalMilliSecond = waitTotalSecond * 1000
                    do! Async.Sleep waitTotalMilliSecond
                    let! response = Twitter.unretweetAndRetweet token tweetId
                    do Console.WriteLine("{0} : {1} にリツイートしました。ID : {2}", name, DateTime.Now, response.Id)
        with
        | e -> do Console.Error.WriteLine e
    }

let configToRetweet (consumerKey : ConsumerKey) (accessToken : AccessToken) (config : Config) =
    let random = MathNet.Numerics.Random.Random.mersenneTwister()
    let token = Twitter.authenticate consumerKey accessToken
    config.Retweets |> Seq.truncate 10 |> Seq.map (getRetweet random token) |> Async.Parallel |> Async.Ignore
