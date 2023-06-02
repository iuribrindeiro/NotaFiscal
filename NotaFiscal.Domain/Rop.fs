[<AutoOpen>]
module Rop


type OperationResult<'TSuccess, 'TErrorMessage> =
    //Mesmo que a operacao tenha resultado em um sucesso, a operacao anterior pode ter falhado,
    //nesse caso, queremos preservar as mensagems de erros anteriores.
    //Por isso temos 'TMessage list em sucesso
    | Success of 'TSuccess * 'TErrorMessage list
    | Failure of 'TErrorMessage list


let succeed x = Success(x, [])

let fail msg = Failure([ msg ])

let failures msgs = Failure(msgs)

let bindR
    (result: OperationResult<'a, 'b>)
    (f: 'a -> OperationResult<'c, 'b>)
    : OperationResult<'a, 'b>
    =
    match result with
    | Success(v, b) ->
        let result' = f v

        match result' with
        | Success _ -> Success(v, b)
        | Failure(msg') -> Failure(msg')
    | Failure(msgs) -> Failure(msgs)

let mapR f result =
    match result with
    | Success(v, msg) -> Success(f v, msg)
    | Failure(msgs) -> Failure(msgs)

let applyR f result =
    match f, result with
    | Success(f, msgs1), Success(x, msgs2) -> (f x, msgs1 @ msgs2) |> Success
    | Failure errs, Success(_, msgs)
    | Success(_, msgs), Failure errs -> errs @ msgs |> Failure
    | Failure errs1, Failure errs2 -> errs1 @ errs2 |> Failure


let mapFailuresR f result =
    match result with
    | Success(v, msg) ->
        let msgs = List.map f msg

        Success(v, msgs)
    | Failure(msgs) ->
        let msgs = List.map f msgs

        Failure(msgs)

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

let mapFailureResults results =
    List.map
        (function
        | Success _ -> []
        | Failure err -> err)
        results
    |> List.concat

type ResultBuilder() =
    member this.Return x = succeed x
    member this.Bind(xResult, f) = bindR xResult f

let result = ResultBuilder()