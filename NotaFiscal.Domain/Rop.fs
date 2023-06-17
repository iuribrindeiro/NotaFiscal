namespace NotaFiscal.Domain

open System.Threading.Tasks

[<AutoOpen>]
module Rop =

    open System


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
        result
        f
        : OperationResult<'a, 'b>
        =
        match result with
        | Success(v, b) ->
            let result' = f v

            match result' with
            | Success (v, r) -> Success(v, r)
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
            
    let mapFailureResults results =
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
                option
                |> Option.map Nullable
                |> Option.defaultWith Nullable
                
    
    [<AutoOpen>]
    module AsyncRop =
        let bindRAsync (result: OperationResult<'a,'b>) (f: 'a -> Task<OperationResult<'c,'b>>) =
            match result with
            | Success(v, _) ->
                task {
                    let! result' = f v

                    return match result' with
                            | Success (v, r) -> Success(v, r)
                            | Failure(msg') -> Failure(msg')   
                }
            | Failure(msgs) -> Failure(msgs) |> Task.FromResult
            
        let mapFailuresRAsync (f: 'a -> 'b) (result: Task<OperationResult<'c,'a>>): Task<OperationResult<'c,'b>> =
            task {
                let! taskResult = result
                return mapFailuresR f taskResult   
            }
            
        let (>>=) = bindRAsync
        
