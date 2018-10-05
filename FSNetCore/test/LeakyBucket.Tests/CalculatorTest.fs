module CalculatorTest

open System
open Xunit
open LeakyBucket
open Xunit.Abstractions

let timeZero () = 0UL
let constantTime x = fun () -> x

[<Fact>]
let ``Returns empty bucket for empty bucket with no time passed`` () =
    let actual = take timeZero { LastState = (0u, 0UL); RefillRate = 2UL }
    Assert.Equal(0u, actual.LastState |> fst)

[<Fact>]
let ``Returns empty bucket for empty bucket with not enough time passed`` () =
    let actual = take (constantTime 1UL) { LastState = (0u, 0UL); RefillRate = 2UL }
    Assert.Equal(0u, actual.LastState |> fst)

[<Fact>]
let ``Returns empty bucket for empty bucket with time passed only for single unit refill`` () =
    let actual = take (constantTime 2UL) { LastState = (0u, 0UL); RefillRate = 2UL }
    Assert.Equal(0u, actual.LastState |> fst)

[<Fact>]
let ``Returns bucket with one unit of water less for non-empty bucket`` () =
    let actual = take timeZero { LastState = (1u, 0UL); RefillRate = 2UL }
    Assert.Equal(0u, actual.LastState |> fst)

[<Fact>]
let ``Returns bucket with time as of now`` () =
    let actual = take (constantTime 42UL) { LastState = (1u, 1UL); RefillRate = 2UL }
    Assert.Equal(42UL, actual.LastState |> snd)

[<Fact>]
let ``Returns bucket with replenished amount of water`` () =
    let actual = take (constantTime 5UL) { LastState = (0u, 0UL); RefillRate = 1UL }
    Assert.Equal(4u, actual.LastState |> fst)

[<Fact>]
let ``Returns emnpty bucket eventually`` () =
    let takeAtTheSameTime = take (constantTime 4UL)
    let actual = { LastState = (1u, 0UL); RefillRate = 2UL }
                 |> takeAtTheSameTime
                 |> takeAtTheSameTime
                 |> takeAtTheSameTime
    Assert.Equal(0u, actual.LastState |> fst)

[<Fact>]
let ``Taking from empty will still leave empty`` () =
    let takeAtTheSameTime = take (constantTime 4UL)
    let actual = { LastState = (0u, 0UL); RefillRate = 2UL }
                 |> takeAtTheSameTime
                 |> takeAtTheSameTime
                 |> takeAtTheSameTime
    Assert.Equal(0u, actual.LastState |> fst)

[<Fact>]
let ``Won't overflow over uint32 `` () =
    let actual = take (constantTime UInt64.MaxValue) { LastState = (0u, 0UL); RefillRate = 1UL }
    Assert.Equal(UInt32.MaxValue - 1u, actual.LastState |> fst)