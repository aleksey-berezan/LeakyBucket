namespace LeakyBucket

type Rate = uint64 // units of time to refill one unit of water
type BucketState = (uint32 * uint64) // remaining * timeStamp
type Bucket = { LastState: BucketState; RefillRate: Rate }

type now = unit -> uint64

[<AutoOpen>]
module Calculator =
    let take (now : now) (bucket : Bucket) : Bucket = 
        let ts = now ()
        let (lastCount, lastAsOf) = bucket.LastState
        let gained = ((ts - lastAsOf) / bucket.RefillRate) |> uint32
        match (lastCount, gained) with
        | (0u, 0u) -> { bucket with LastState = (0u, ts) }
        | _ -> let current = lastCount + gained - 1u
               { bucket with LastState = (current, ts) }
