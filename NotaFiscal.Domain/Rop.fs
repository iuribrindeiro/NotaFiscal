[<AutoOpen>]
module Rop


type OperationResult<'TSuccess, 'TErrorMessage> =
    //Mesmo que a operacao tenha resultado em um sucesso, a operacao anterior pode ter falhado,
    //nesse caso, queros preservar as mensagems de erros anteriores.
    //Por isso temos 'TMessage list em sucesso
    | Success of 'TSuccess * 'TErrorMessage list
    | Failure of 'TErrorMessage list


let succeed x = Success(x, [])

let fail msg = Failure([ msg ])


let bind f result =
    match result with
    | Success(v, msg) ->
        let result' = f (v, msg)

        match result' with
        | Success(v', msg') -> Success(v', msg @ msg')
        | Failure(msg') -> Failure(msg @ msg')
    | Failure(msgs) -> Failure(msgs)

let map f result =
    match result with
    | Success(v, msg) -> Success(f v, msg)
    | Failure(msgs) -> Failure(msgs)

let apply f result =
    match f, result with
    | Success(f, msgs1), Success(x, msgs2) -> (f x, msgs1 @ msgs2) |> Success
    | Failure errs, Success(_, msgs)
    | Success(_, msgs), Failure errs -> errs @ msgs |> Failure
    | Failure errs1, Failure errs2 -> errs1 @ errs2 |> Failure


let mapFailures f result =
    match result with
    | Success(v, msg) ->
        let msgs = List.map f msg
        Success(v, msgs)
    | Failure(msgs) ->
        let msgs = List.map f msgs
        Failure(msgs)

let (<*>) = apply
let (<!>) = map

let traverseResult (f) (value) =
    match value with
    | None -> succeed None
    | Some x -> (succeed Some) <*> (f x)


let mapListOptionToList (values: 'a option list) =
    let rec mapListOptionToList' (values: 'a option list) (acc: 'a list) =
        match values with
        | [] -> List.rev acc
        | Some x :: xs -> mapListOptionToList' xs (x :: acc)
        | None :: xs -> mapListOptionToList' xs acc

    mapListOptionToList' values []
