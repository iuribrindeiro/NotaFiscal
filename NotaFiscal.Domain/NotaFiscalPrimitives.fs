module NotaFiscal.Domain.NotaFiscalPrimitives

open System
open System.Text.RegularExpressions

type Result<'TSuccess, 'TMessage> =
    | Success of 'TSuccess * 'TMessage list
    | Failure of 'TMessage list

type StringError =
    | Missing
    | DoesntMatchPattern
    | MustNotBeLongerThan of int
    | MustHaveLen of int

type DinheiroErrors =
    | Missing
    | MustBeGreaterThan0

type PercentageError = | MustBeBetween0And100

let succeed x = Success(x, [])

let fail msg = Failure([ msg ])


let createMaxStringLen len value f =
    match value with
    | v when String.IsNullOrWhiteSpace v -> fail StringError.Missing
    | v when v.Length > len -> fail (StringError.MustNotBeLongerThan len)
    | v -> succeed (f v)


let stringOfLen len value f =
    match value with
    | v when String.IsNullOrWhiteSpace v -> fail StringError.Missing
    | v when v.Length <> len -> fail (StringError.MustHaveLen len)
    | v -> succeed (f v)

module StringMax5 =
    type Value = StringMax5 of string

    let create value = createMaxStringLen 5 value StringMax5

module StringMax20 =
    type Value = StringMax20 of string

    let create value = createMaxStringLen 20 value StringMax20

module StringMax2000 =
    type Value = StringMax2000 of string

    let create value =
        createMaxStringLen 2000 value StringMax2000


module StringMax7 =
    type Value = StringMax7 of string

    let create value = createMaxStringLen 7 value StringMax7

module StringMax60 =
    type Value = String60 of string

    let create value = createMaxStringLen 60 value String60

module StringMax120 =
    type Value = String120 of string

    let create value = createMaxStringLen 120 value String120

module StringMax15 =
    type Value = StringMax15 of string

    let create value = createMaxStringLen 15 value StringMax15

module StringMax115 =
    type Value = StringMax115 of string

    let create value =
        createMaxStringLen 115 value StringMax115

module StringMax11 =
    type Value = StringMax11 of string

    let create value = createMaxStringLen 11 value StringMax11

module String8 =
    type Value = String8 of string

    let create value = stringOfLen 8 value String8

module StringMax80 =
    type Value = StringMax80 of string

    let create value = stringOfLen 80 value StringMax80

module Dinheiro =
    type Value = Dinheiro of decimal

    let create value =
        match value with
        | v when v = 0m -> fail DinheiroErrors.Missing
        | v when v < 0m -> fail DinheiroErrors.MustBeGreaterThan0
        | v -> succeed (Dinheiro v)

module EmailAddress =
    type Value = EmailAddress of string

    let create value =
        match value with
        | v when String.IsNullOrWhiteSpace v -> fail StringError.Missing
        | v when String.length v > 80 -> fail (StringError.MustNotBeLongerThan 80)
        | v when not (Regex.IsMatch(v, "^[^@]+@[^@]+\\.[^@]+$")) -> fail StringError.DoesntMatchPattern
        | v -> succeed (EmailAddress v)

module CNPJ =
    type Value = CNPJ of string

    let create value =
        match value with
        | v when String.IsNullOrWhiteSpace v -> fail StringError.Missing
        | _ ->
            let cnpjNumeros = Regex.Replace(value, "\\D", "")

            let error = fail StringError.DoesntMatchPattern

            match cnpjNumeros with
            | s when String.length s <> 14 -> error
            | _ ->
                let pesos = [ 5; 4; 3; 2; 9; 8; 7; 6; 5; 4; 3; 2 ]

                let calcularDigito inicio fim =
                    [ inicio..fim ]
                    |> List.map (fun i -> int (cnpjNumeros.[i].ToString()) * pesos.[i - inicio])
                    |> List.sum
                    |> fun soma -> if soma % 11 < 2 then 0 else 11 - soma % 11

                let digito1 = calcularDigito 0 11
                let digito2 = calcularDigito 0 12

                if
                    digito1 = int (cnpjNumeros.[12].ToString())
                    && digito2 = int (cnpjNumeros.[13].ToString())
                then
                    succeed (CNPJ value)
                else
                    error

module CPF =
    type Value = CPF of string

    let create value =
        match value with
        | v when String.IsNullOrWhiteSpace v -> fail StringError.Missing
        | _ ->
            let cpfNumeros = Regex.Replace(value, "\\D", "")

            let error = fail StringError.DoesntMatchPattern

            match cpfNumeros with
            | s when String.length s <> 11 -> error
            | _ ->
                let pesos = [ 10; 9; 8; 7; 6; 5; 4; 3; 2 ]

                let calcularDigito inicio fim =
                    [ inicio..fim ]
                    |> List.map (fun i -> int (cpfNumeros.[i].ToString()) * pesos.[i - inicio])
                    |> List.sum
                    |> fun soma -> if soma % 11 < 2 then 0 else 11 - soma % 11

                let digito1 = calcularDigito 0 9
                let digito2 = calcularDigito 0 10

                if
                    digito1 = int (cpfNumeros.[9].ToString())
                    && digito2 = int (cpfNumeros.[10].ToString())
                then
                    succeed (CPF value)
                else
                    error

module UF =
    type Value =
        | AC
        | AL
        | AM
        | AP
        | BA
        | CE
        | DF
        | ES
        | GO
        | MA
        | MG
        | MS
        | MT
        | PA
        | PB
        | PE
        | PI
        | PR
        | RJ
        | RN
        | RO
        | RR
        | RS
        | SC
        | SE
        | SP
        | TO

    let create (value: string) =
        match value with
        | v when String.IsNullOrWhiteSpace v -> fail StringError.Missing
        | "AC" -> succeed AC
        | "AL" -> succeed AL
        | "AP" -> succeed AP
        | "AM" -> succeed AM
        | "BA" -> succeed BA
        | "CE" -> succeed CE
        | "DF" -> succeed DF
        | "ES" -> succeed ES
        | "GO" -> succeed GO
        | "MA" -> succeed MA
        | "MT" -> succeed MT
        | "MS" -> succeed MS
        | "MG" -> succeed MG
        | "PA" -> succeed PA
        | "PB" -> succeed PB
        | "PR" -> succeed PR
        | "PE" -> succeed PE
        | "PI" -> succeed PI
        | "RJ" -> succeed RJ
        | "RN" -> succeed RN
        | "RS" -> succeed RS
        | "RO" -> succeed RO
        | "RR" -> succeed RR
        | "SC" -> succeed SC
        | "SP" -> succeed SP
        | "SE" -> succeed SE
        | "TO" -> succeed TO
        | _ -> fail StringError.DoesntMatchPattern

module Percentage =
    type Value = Percentage of float

    let create value =
        if value < 0.0 || value > 100.0 then
            fail PercentageError.MustBeBetween0And100
        else
            succeed (Percentage value)
