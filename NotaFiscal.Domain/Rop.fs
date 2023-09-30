[<AutoOpen>]
module NotaFiscal.Domain.Rop

open System.Threading.Tasks
open System

[<AutoOpen>]
module Rop =


    type OperationResult<'TSuccess, 'TErrorMessage> =
        | Success of 'TSuccess * 'TErrorMessage list
        | Failure of 'TErrorMessage list



    let succeed x = Success(x, [])

    let fail msg = Failure([ msg ])

    let failures msgs = Failure(msgs)

    let teeR f result =
        match result with
        | Success(v, msgs) ->
            let result' = f v

            match result' with
            | Success(v, r) -> Success(v, r @ msgs)
            | Failure(msg') -> Failure(msg' @ msgs)
        | Failure(msgs) -> Failure(msgs)

    let bindR result f : OperationResult<'a, 'b> =
        match result with
        | Success(v, msgs) ->
            let result' = f v

            match result' with
            | Success(v, r) -> Success(v, r @ msgs)
            | Failure(msg') -> Failure(msg' @ msgs)
        | Failure(msgs) -> Failure(msgs)

    let mapR f result =
        match result with
        | Success(v, msg) -> Success(f v, msg)
        | Failure(msgs) -> Failure(msgs)

    let mapMsgR f result =
        match result with
        | Success(v, msgs) -> Success(v, [ f v ] @ msgs)
        | Failure(msgs) -> Failure(msgs)

    let applyR
        (resultOfFunc: OperationResult<('a -> 'b), 'c>)
        (result: OperationResult<'a, 'c>)
        : OperationResult<'b, 'c>
        =
        match resultOfFunc, result with
        | Success(f, msgs1), Success(x, msgs2) ->
            (f x, msgs1 @ msgs2) |> Success
        | Failure errs, Success(_, msgs)
        | Success(_, msgs), Failure errs -> errs @ msgs |> Failure
        | Failure errs1, Failure errs2 -> errs1 @ errs2 |> Failure

    let rec traverseResultA
        (f: 'a -> OperationResult<'b, 'c>)
        (list: 'a list)
        : OperationResult<'b list, 'c>
        =
        // define the applicative functions
        let (<*>) = applyR

        let retn =
            function
            | x -> Success(x, [])

        // define a "cons" function
        let cons head tail = head :: tail

        // loop through the list
        match list with
        | [] ->
            // if empty, lift [] to a Result
            retn []
        | head :: tail ->
            // otherwise lift the head to a Result using f
            // and cons it with the lifted version of the remaining list
            retn cons <*> (f head) <*> (traverseResultA f tail)

    let mapFailuresR
        (f: 'a -> 'b)
        (result: OperationResult<'c, 'a>)
        : OperationResult<'c, 'b>
        =
        match result with
        | Success(v, msg) ->
            let msgs = List.map f msg

            Success(v, msgs)
        | Failure(msgs) ->
            let msgs = List.map f msgs
            Failure(msgs)

    let aggregateFailuresR (results: OperationResult<'a, 'b> list) : 'b list =
        List.map
            (function
            | Success _ -> []
            | Failure err -> err)
            results
        |> List.concat

    let strToOptionStr value =
        match value with
        | v when String.IsNullOrWhiteSpace v -> None
        | _ -> Some value

    let (<*>) = applyR
    let (<!>) = mapR
    let (>>=) = bindR

    let isFailure result =
        match result with
        | Success _ -> false
        | Failure(_) -> true

    let toResult fResult err =
        match fResult with
        | Ok v -> succeed v
        | Error _ -> fail err

    let hasAnyFailure results = results |> List.exists isFailure

    let failIfNoneR msg value =
        match value with
        | None -> fail msg
        | Some x -> succeed x

    let traverseResult f value =
        match value with
        | None -> succeed None
        | Some x -> (succeed Some) <*> (f x)

    let mapSuccessResults results =
        List.map
            (function
            | Success(v, _) -> [ v ]
            | Failure(_) -> [])
            results
        |> List.concat

    let mapNullToOptR value f =
        match box value with
        | null -> succeed None
        | _ -> f value

    let mapNullToR value f err =
        match box value with
        | null -> fail err
        | _ -> f value

    type ResultBuilder() =
        member this.Return x = succeed x
        member this.Bind(xResult, f) = bindR xResult f

    let result = ResultBuilder()

    module Option =
        let withDefaultNullable option =
            option |> Option.map Nullable |> Option.defaultWith Nullable


    [<AutoOpen>]
    module AsyncRop =
        let bindRAsync
            (result: OperationResult<'a, 'b>)
            (f: 'a -> Task<OperationResult<'c, 'b>>)
            =
            match result with
            | Success(v, _) ->
                task {
                    let! result' = f v

                    return
                        match result' with
                        | Success(v, r) -> Success(v, r)
                        | Failure(msg') -> Failure(msg')
                }
            | Failure(msgs) -> Failure(msgs) |> Task.FromResult

        let mapRAsync resultRAsync f =
            task {
                let! resultR = resultRAsync

                return mapR f resultR
            }

        let checkRAsync resultR f =
            match resultR with
            | Success(v, msgs) ->
                task {
                    let! result' = f v

                    return
                        match result' with
                        | Success(_, r) -> Success(v, r @ msgs)
                        | Failure(msg') -> Failure(msg' @ msgs)
                }
            | Failure(msgs) -> Failure(msgs) |> Task.FromResult

        let mapFailuresRAsync
            (f: 'a -> 'b)
            (result: Task<OperationResult<'c, 'a>>)
            : Task<OperationResult<'c, 'b>>
            =
            task {
                let! taskResult = result
                return mapFailuresR f taskResult
            }

        let (>>=) = bindRAsync
