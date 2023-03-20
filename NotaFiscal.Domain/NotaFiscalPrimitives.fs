module NotaFiscal.Domain.NotaFiscalPrimitives

open System
open System.Text.RegularExpressions
open Rop
open ValidationErrors

let inline (>=<) a (b, c) = a >= b && a <= c

let createMaxStringLen len value f =
    match value with
    | v when String.IsNullOrWhiteSpace v -> fail MaxLenNotEmpty
    | v when v.Length > len -> fail (NotEmptyMaxLen len)
    | v -> succeed (f v)


let optionalMaxStringLen len value f =
    let maxLenValidation v =
        match v with
        | Some v when String.length v > len -> fail (MaxLen len)
        | Some v when String.IsNullOrWhiteSpace v -> succeed (f None)
        | Some v -> succeed (f (Some v))
        | None -> succeed (f None)

    maxLenValidation value


let stringOfLen len value f =
    match value with
    | v when String.IsNullOrWhiteSpace v -> fail LenNotEmpty
    | v when v.Length <> len -> fail (MustHaveLen len)
    | v -> succeed (f v)

module StringMax5 =
    type Value = StringMax5 of string

    let create value = createMaxStringLen 5 value StringMax5

module StringMax20 =
    type Value = StringMax20 of string

    type OptionalValue = StringMax20Optional of string option

    let create value = createMaxStringLen 20 value StringMax20

    let createOptional value =
        optionalMaxStringLen 20 value StringMax20Optional

module StringMax2000 =
    type Value = StringMax2000 of string

    let create value =
        createMaxStringLen 2000 value StringMax2000


module StringMax7 =
    type Value = StringMax7 of string
    type OptionalValue = StringMax7Optional of string option

    let create value = createMaxStringLen 7 value StringMax7

    let createOptional value =
        optionalMaxStringLen 7 value StringMax7Optional

module StringMax60 =
    type Value = String60 of string
    type OptionalValue = String60Optional of string option

    let create value = createMaxStringLen 60 value String60

    let createOptional value =
        optionalMaxStringLen 60 value String60Optional

module StringMax120 =
    type Value = String120 of string

    let create value = createMaxStringLen 120 value String120

module StringMax15 =
    type Value = StringMax15 of string

    type OptionalValue = StringMax15Optional of string option

    let create value = createMaxStringLen 15 value StringMax15

    let createOptional value =
        optionalMaxStringLen 15 value StringMax15Optional

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

module ValorDinheiro =
    type Value = Dinheiro of decimal

    type OptionalValue = DinheiroOptional of decimal option

    let create value =
        match value with
        | v when v <= 0m -> fail ValorDinheiroMustBePositive
        | v -> succeed (Dinheiro v)

    let createOptional value =
        match value with
        | Some v when v = 0m -> DinheiroOptional None |> succeed
        | Some v when v < 0m -> fail ValorDinheiroMustBePositive
        | Some v -> Some v |> DinheiroOptional |> succeed
        | None -> DinheiroOptional None |> succeed

module EmailAddress =
    type Value = EmailAddress of string


    let create value =
        match value with
        | v when String.IsNullOrWhiteSpace v -> fail MaxLenPatternNotEmpty
        | v when String.length v > 80 -> fail (PatternNotEmptyMaxLen 80)
        | v when not (Regex.IsMatch(v, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) -> fail DoesntMatchPattern
        | v -> succeed (EmailAddress v)

module Telefone =
    type Value = Telefone of string

    let create value =
        match value with
        | v when String.IsNullOrWhiteSpace v -> fail TelefoneNotEmpty
        | v when String.length v >=< (8, 11) -> fail (TelefoneMustBetweenLen(8, 11))
        | v when not (Regex.IsMatch(v, "^[0-9]+$")) -> fail TelefoneDoesntMatchPattern
        | v -> succeed (Telefone v)

module NaturezaOperacao =
    type Value =
        | TributacaoMunicipio
        | TributacaoForaMunicipio
        | Isencao
        | Imune
        | ExigibilidadeSuspensa of NaturezaOperacaoExigibilidadeSuspensa

    and NaturezaOperacaoExigibilidadeSuspensa =
        | DecisaoJudicial
        | ProcedimentoAdministrativo

    let create (value: int) =
        match value with
        | 1 -> succeed TributacaoMunicipio
        | 2 -> succeed TributacaoForaMunicipio
        | 3 -> succeed Isencao
        | 4 -> succeed Imune
        | 5 -> succeed (ExigibilidadeSuspensa DecisaoJudicial)
        | 6 -> succeed (ExigibilidadeSuspensa ProcedimentoAdministrativo)
        | _ -> fail NaturezaOperacaoInvalida

module RegimeEspecialTributacao =
    type Value =
        | MicroempresaMunicipal
        | Estimativa
        | SociedadeProfissionais
        | Cooperativa
        | MicroempreendedorIndividual
        | MicroempreendedorPequenoPorte

    let create (value: int) =
        match value with
        | 1 -> succeed MicroempresaMunicipal
        | 2 -> succeed Estimativa
        | 3 -> succeed SociedadeProfissionais
        | 4 -> succeed Cooperativa
        | 5 -> succeed MicroempreendedorIndividual
        | 6 -> succeed MicroempreendedorPequenoPorte
        | _ -> fail RegimeEspecialTributacaoInvalido

module CNPJ =
    type Value = CNPJ of string

    let create value =
        match value with
        | v when String.IsNullOrWhiteSpace v -> fail NotEmptyCNPJ
        | _ ->
            let cnpjNumeros = Regex.Replace(value, "\\D", "")

            let error = fail DoesntMatchCNPJPattern

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
        | v when String.IsNullOrWhiteSpace v -> fail NotEmptyCPF
        | _ ->
            let cpfNumeros = Regex.Replace(value, "\\D", "")

            let error = fail DoesntMatchCPFPattern

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
        | v when String.IsNullOrWhiteSpace v -> fail NotEmptyUF
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
        | _ -> fail DoesntUFMatchPattern

module Percentage =
    type Value = Percentage of double

    type OptionalValue = PercentageOptional of double option

    let create value =
        if value < 0.0 || value > 100.0 then
            fail PercentageError.MustBeBetween0And100
        else
            succeed (Percentage value)

    let createOptional value =
        match value with
        | Some v when v = 0.0 -> PercentageOptional None |> succeed
        | Some v when v < 0.0 || v > 100.0 -> fail PercentageError.MustBeBetween0And100
        | _ -> PercentageOptional None |> succeed
