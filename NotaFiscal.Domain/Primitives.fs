[<AutoOpen>]
module NotaFiscal.Domain.Primitives

open System
open System.Text.RegularExpressions

[<AutoOpen>]
module Primitives =

    let inline (>=<) a (b, c) = a >= b && a <= c

    type StringError =
        | Missing
        | MustNotBeLongerThan of int
        | MustHaveLen of int
        | MustBeBetween of int * int
        | DoesntMatchPattern of string

    type DateError =
        | Missing
        | MustBePast

    type NumberError =
        | Missing
        | MustBePositive
        | MustBeBetween of int * int

    type EnumError<'a> = | Invalid of 'a

    type StrMax20 = private | StrMax20 of string
    type MaybeStrMax20 = private | MaybeStrMax20 of string option

    type StrMax2000 = private | StrMax2000 of string

    type StrMax7 = private | StrMax7 of string
    type StrOf2 = private | StrOf2 of string
    type MaybeStrMax7 = private | MaybeStrMax7 of string option

    type StrMax60 = private | StrMax60 of string
    type MaybeStrMax60 = private | MaybeStrMax60 of string option

    type StrMax120 = private | StrMax120 of string
    type MaybeStrMax120 = private | MaybeStrMax120 of string option

    type StrMax15 = private | StrMax15 of string
    type MaybeStrMax15 = private | MaybeStrMax15 of string option

    type StrMax115 = private | StrMax115 of string
    type MaybeStrMax115 = private | MaybeStrMax115 of string option

    type StrMax11 = private | StrMax11 of string

    type StrOf8 = private | StrOf8 of string

    type StrMax80 = private | StrMax80 of string

    type StrMax5 = private | StrMax5 of string

    type NotEmptyStr = private | NotEmptyStr of string

    type Dinheiro = private | Dinheiro of decimal
    type MaybeDinheiro = private | MaybeDinheiro of decimal option

    type Telefone = private | Telefone of string
    type EmailAddress = private | EmailAddress of StrMax80

    type PositiveInt = private | PositiveInt of int

    type PastDate = private | PastDate of DateTime

    type TipoRps =
        private
        | Rps
        | NotaFiscalConjugadaMista
        | Cupom

    type NaturezaOperacao =
        private
        | TributacaoMunicipio
        | TributacaoForaMunicipio
        | Isencao
        | Imune
        | ExigibilidadeSuspensa of NaturezaOperacaoExigibilidadeSuspensa

    and NaturezaOperacaoExigibilidadeSuspensa =
        private
        | DecisaoJudicial
        | ProcedimentoAdministrativo

    type RegimeEspecialTributacao =
        private
        | MicroempresaMunicipal
        | Estimativa
        | SociedadeProfissionais
        | Cooperativa
        | MicroempreendedorIndividual
        | MicroempreendedorPequenoPorte

    type CNPJ = private | CNPJ of string
    type CPF = private | CPF of string

    type MaybePercentage = private | MaybePercentage of double option

    let notEmpty (value: string) : OperationResult<string, StringError> =
        if String.IsNullOrWhiteSpace value then
            fail StringError.Missing
        else
            succeed value

    let notEmptyDate (value: DateTime) : OperationResult<DateTime, DateError> =
        if value = DateTime.MinValue then
            fail DateError.Missing
        else
            succeed value

    let isPastDate (value: DateTime) : OperationResult<DateTime, DateError> =
        if value < DateTime.Now then
            succeed value
        else
            fail DateError.MustBePast

    let hasMaxLen
        (len: int)
        (value: string)
        : OperationResult<string, StringError>
        =
        if String.length value > len then
            MustNotBeLongerThan len |> fail
        else
            succeed value

    let isBetween
        (start: int)
        (end': int)
        (value: string)
        : OperationResult<string, StringError>
        =
        if value |> String.length >=< (start, end') |> not then
            StringError.MustBeBetween(start, end') |> fail
        else
            succeed value

    let isOfLen
        (len: int)
        (value: string)
        : OperationResult<string, StringError>
        =
        if String.length value <> len then
            MustHaveLen len |> fail
        else
            succeed value

    let matchesPattern
        (pattern: string)
        (value: string)
        : OperationResult<string, StringError>
        =
        if Regex.IsMatch(value, pattern) |> not then
            StringError.DoesntMatchPattern value |> fail
        else
            succeed value

    let notZeroInt value =
        if value = 0 |> not then
            succeed value
        else
            fail NumberError.Missing

    let notZeroDecimal value =
        if value = 0m |> not then
            succeed value
        else
            fail NumberError.Missing

    let notNegativeDecimal value =
        if value >= 0m |> not then
            succeed value
        else
            fail NumberError.MustBePositive

    let validateRules
        (value: 'a)
        (f: 'a -> 'b)
        (validators: ('a -> OperationResult<'a, 'c>) list)
        : OperationResult<'b, 'c>
        =
        let errors =
            validators
            |> List.map (fun x -> x value)
            |> aggregateFailuresR

        match errors with
        | [] -> f value |> succeed
        | _ -> failures errors

    let createMaxStringLen len value f =
        [ notEmpty
          hasMaxLen len ]
        |> validateRules value f

    let optionalMaxStringLen
        (len: int)
        (value: string option)
        (f: string option -> 'a)
        : OperationResult<'a, StringError>
        =
        match value with
        | Some v when String.IsNullOrWhiteSpace v -> f None |> succeed
        | Some v -> Some >> f <!> hasMaxLen len v
        | None -> f None |> succeed

    let stringOfLen len value f =
        [ notEmpty
          isOfLen len ]
        |> validateRules value f

    module StrMax20 =
        let create value = createMaxStringLen 20 value StrMax20

        let createOptional value =
            optionalMaxStringLen 20 value MaybeStrMax20

        let mapToValue (StrMax20 value) = value

        let mapToValueOption (MaybeStrMax20 value) = value

    module StrMax2000 =
        let create value =
            createMaxStringLen 2000 value StrMax2000

        let mapToValue (StrMax2000 value) = value

    module StrMax5 =
        let create value = createMaxStringLen 5 value StrMax5

        let mapToValue (StrMax5 value) = value

    module StrMax7 =
        let create value = createMaxStringLen 7 value StrMax7

        let createOptional value =
            optionalMaxStringLen 7 value MaybeStrMax7

        let mapToValue (StrMax7 value) = value

        let mapToValueOptional (MaybeStrMax7 value) = value

    module StrOf2 =
        let create value = stringOfLen 2 value StrOf2

        let mapToValue (StrOf2 value) = value

    module StrMax60 =
        let create value = createMaxStringLen 60 value StrMax60

        let createOptional
            (value: string option)
            : OperationResult<MaybeStrMax60, StringError>
            =
            optionalMaxStringLen 60 value MaybeStrMax60

        let mapToValue (StrMax60 value) = value

        let mapToValueOptional (MaybeStrMax60 value) = value

    module StrMax120 =
        let create value = createMaxStringLen 120 value StrMax120

        let mapToValue (StrMax120 value) = value

        let createOptional value =
            optionalMaxStringLen 120 value MaybeStrMax120

        let mapToValueOptional (MaybeStrMax120 value) = value

    module StrMax15 =
        let create value = createMaxStringLen 15 value StrMax15

        let createOptional value =
            optionalMaxStringLen 15 value MaybeStrMax15

        let mapToValueOptional (MaybeStrMax15 value) = value

        let mapToValue (StrMax15 value) = value

    module StrMax115 =
        let create value = createMaxStringLen 115 value StrMax115

        let createOptional value =
            optionalMaxStringLen 115 value MaybeStrMax115

        let mapToValue (StrMax115 value) = value

        let mapToValueOptional (MaybeStrMax115 value) = value

    module StrMax11 =
        let create value = createMaxStringLen 11 value StrMax11

    module StrOf8 =
        let create value = stringOfLen 8 value StrOf8

        let mapToValue (StrOf8 value) = value

    module StrMax80 =
        let create value = stringOfLen 80 value StrMax80

    module NotEmptyStr =
        let create value =
            [ notEmpty ] |> validateRules value NotEmptyStr

        let mapToValue (NotEmptyStr value) = value

    module PastDate =
        let create value =
            [ notEmptyDate
              isPastDate ]
            |> validateRules value PastDate

        let mapToValue (PastDate value) = value

    module PositiveInt =
        let create value =
            [ notZeroInt ] |> validateRules value PositiveInt

        let mapToValue (PositiveInt value) = value

    module Dinheiro =
        let create value =
            [ notZeroDecimal
              notNegativeDecimal ]
            |> validateRules value Dinheiro

        let createOptional value =
            match value with
            | None -> MaybeDinheiro None |> succeed
            | Some v when v = 0m -> MaybeDinheiro None |> succeed
            | Some v when v < 0m -> fail MustBePositive
            | Some v -> Some v |> MaybeDinheiro |> succeed

        let mapToValue (Dinheiro value) = value

        let mapToValueOptional (MaybeDinheiro value) = value

    module EmailAddress =
        let create value =
            [ notEmpty
              matchesPattern "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
              hasMaxLen 80 ]
            |> validateRules value (StrMax80 >> EmailAddress)

        let mapToValue (EmailAddress(StrMax80 value)) = value

    module Telefone =
        let create value =
            [ notEmpty
              isBetween 8 11
              matchesPattern "^[0-9]+$" ]
            |> validateRules value Telefone

        let mapValue (Telefone value) = value

    module TipoRps =
        let create: int -> OperationResult<TipoRps, EnumError<int>> =
            function
            | 1 -> succeed Rps
            | 2 -> succeed NotaFiscalConjugadaMista
            | 3 -> succeed Cupom
            | e -> Invalid e |> fail

        let mapValue =
            function
            | Rps -> 1
            | NotaFiscalConjugadaMista -> 2
            | Cupom -> 3

    module NaturezaOperacao =
        let create =
            function
            | 1 -> succeed TributacaoMunicipio
            | 2 -> succeed TributacaoForaMunicipio
            | 3 -> succeed Isencao
            | 4 -> succeed Imune
            | 5 -> succeed (ExigibilidadeSuspensa DecisaoJudicial)
            | 6 -> succeed (ExigibilidadeSuspensa ProcedimentoAdministrativo)
            | e -> Invalid e |> fail

        let mapValue =
            function
            | TributacaoMunicipio -> 1
            | TributacaoForaMunicipio -> 2
            | Isencao -> 3
            | Imune -> 4
            | ExigibilidadeSuspensa DecisaoJudicial -> 5
            | ExigibilidadeSuspensa ProcedimentoAdministrativo -> 6

    module RegimeEspecialTributacao =
        let create =
            function
            | 1 -> succeed MicroempresaMunicipal
            | 2 -> succeed Estimativa
            | 3 -> succeed SociedadeProfissionais
            | 4 -> succeed Cooperativa
            | 5 -> succeed MicroempreendedorIndividual
            | 6 -> succeed MicroempreendedorPequenoPorte
            | e -> Invalid e |> fail

        let mapValue =
            function
            | MicroempresaMunicipal -> 1
            | Estimativa -> 2
            | SociedadeProfissionais -> 3
            | Cooperativa -> 4
            | MicroempreendedorIndividual -> 5
            | MicroempreendedorPequenoPorte -> 6

    module CNPJ =
        let create =
            function
            | v when String.IsNullOrWhiteSpace v -> fail StringError.Missing
            | value ->
                let cnpjNumeros =
                    Regex.Replace(value, "\\D", "")

                let error =
                    StringError.DoesntMatchPattern value |> fail

                match cnpjNumeros with
                | s when String.length s <> 14 -> error
                | _ ->
                    let pesos =
                        [ 5
                          4
                          3
                          2
                          9
                          8
                          7
                          6
                          5
                          4
                          3
                          2 ]

                    let calcularDigito inicio fim =
                        [ inicio..fim ]
                        |> List.map (fun i ->
                            int (
                                cnpjNumeros[i]
                                    .ToString()
                            )
                            * pesos[i - inicio])
                        |> List.sum
                        |> fun soma ->
                            if soma % 11 < 2 then 0 else 11 - soma % 11

                    let digito1 = calcularDigito 0 11
                    let digito2 = calcularDigito 0 12

                    if
                        digito1 = int (
                            cnpjNumeros[12]
                                .ToString()
                        )
                        && digito2 = int (
                            cnpjNumeros[13]
                                .ToString()
                        )
                    then
                        succeed (CNPJ value)
                    else
                        error

        let mapToValue (CNPJ value) = value

    module CPF =
        let create =
            function
            | v when String.IsNullOrWhiteSpace v -> fail StringError.Missing
            | value ->
                let cpfNumeros =
                    Regex.Replace(value, "\\D", "")

                let error =
                    StringError.DoesntMatchPattern value |> fail

                match cpfNumeros with
                | s when String.length s <> 11 -> error
                | _ ->
                    let pesos =
                        [ 10
                          9
                          8
                          7
                          6
                          5
                          4
                          3
                          2 ]

                    let calcularDigito inicio fim =
                        [ inicio..fim ]
                        |> List.map (fun i ->
                            int (
                                cpfNumeros[i]
                                    .ToString()
                            )
                            * pesos[i - inicio])
                        |> List.sum
                        |> fun soma ->
                            if soma % 11 < 2 then 0 else 11 - soma % 11

                    let digito1 = calcularDigito 0 9
                    let digito2 = calcularDigito 0 10

                    if
                        digito1 = int (
                            cpfNumeros[9]
                                .ToString()
                        )
                        && digito2 = int (
                            cpfNumeros[10]
                                .ToString()
                        )
                    then
                        succeed (CPF value)
                    else
                        error

        let mapToValue (CPF value) = value

    module Percentage =
        let createOptional
            : float option -> OperationResult<MaybePercentage, NumberError> =
            function
            | Some v when v = 0.0 -> MaybePercentage None |> succeed
            | Some v when v < 0.0 || v > 100.0 -> MustBeBetween(0, 100) |> fail
            | Some v -> MaybePercentage(Some v) |> succeed
            | None -> MaybePercentage None |> succeed


        let mapToValueOptional (MaybePercentage value) = value
