module Adacola.SoftOmeletRice.Twitter

open CoreTweet
open CoreTweet.CoreTweetFSharp
open System

let inline (!?) x = Nullable x

let authenticate (consumerKey : ConsumerKey) (accessToken : AccessToken) =
    Tokens.Create(consumerKey.ConsumerKey, consumerKey.ConsumerSecret, accessToken.AccessToken, accessToken.AccessTokenSecret)

let unretweet (token : Tokens) (TweetID tweetID) =
    async {
        return! token.Statuses.UnretweetAsync tweetID |> Async.AwaitTask
    }
    
let retweet (token : Tokens) (TweetID tweetID) =
    async {
        return! token.Statuses.RetweetAsync tweetID |> Async.AwaitTask
    }

let unretweetAndRetweet (token : Tokens) tweetID =
    async {
        do! unretweet token tweetID |> Async.Ignore
        do! Async.Sleep 1000
        return! retweet token tweetID
    }
