namespace Adacola.SoftOmeletRice

[<AutoOpen>]
module Model =
    open FSharp.Data

    type ConsumerKeyProvider = JsonProvider<"consumerKeySample.json">
    type ConsumerKey = ConsumerKeyProvider.Root
    type AccessTokenProvider = JsonProvider<"accessTokenSample.json">
    type AccessToken = AccessTokenProvider.Root
    type ConfigProvider = JsonProvider<"configSample.json">
    type Config = ConfigProvider.Root
    type TweetID = TweetID of int64