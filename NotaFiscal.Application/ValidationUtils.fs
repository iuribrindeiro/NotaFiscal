module ValidationUtils

open System
open System.Text.RegularExpressions

type CustomResult<'a> = Result<'a, string list>
type ValidationResult<'a> = ((string * 'a option) * CustomResult<'a>)
type Validator<'a> = ((string * 'a option) -> ValidationResult<'a>)

let composeValidator ((fieldName: string, value: 'b option), result: CustomResult<'a>) (validator: Validator<'b>) =
    match result with
    | Ok _ -> validator (fieldName, value)
    | Error errors ->
        let (nextField, nextResult) = validator (fieldName, value)

        match nextResult with
        | Ok _ -> (nextField, nextResult)
        | Error newErrors -> ((fieldName, value), Error(errors @ newErrors))

//let (>>=) = composeValidator


let mapOptionalResultValue (result: Result<'a, string list>) =
    match result with
    | Ok value -> Ok(Some value)
    | Error err -> Error(err)

let mapResult (validationResult: ValidationResult<'a>) =
    let (_, result) = validationResult

    match result with
    | Ok value -> Ok value
    | Error errors -> Error errors

let validOption (fieldName, value) =
    match value with
    | Some value -> Ok value
    | None -> Error [ sprintf "O campo %s não é válido" fieldName ]

let requiredField (fieldName: string) (validator: Validator<'a>) (value: 'a option) =
    let isRequired (fieldName, value) : ValidationResult<'a> =
        let errMsg = Error([ sprintf "O campo %s não pode ser vazio" fieldName ])

        match value with
        | Some v -> (fieldName, value), Ok v
        | None -> (fieldName, value), errMsg

    match value with
    | Some _ -> (fieldName, value) |> validator |> mapResult
    | None -> (fieldName, value) |> isRequired >>= validator |> mapResult

let optionalField (fieldName: string) (validator: Validator<'a>) (value: 'a option) =
    match value with
    | Some _ -> requiredField fieldName validator value |> mapOptionalResultValue
    | None -> Ok None

let optionalStrField (fieldName: string) (validator: Validator<string>) (value: string option) =
    match value with
    | Some v when not (String.IsNullOrWhiteSpace v) -> requiredField fieldName validator value |> mapOptionalResultValue
    | _ -> Ok None



let inline (><) value (min, max) = value < min && value > max

let inline (><!) value (min, max) = value > min || value < max

let runValidation
    (fieldName: string, value: 'a option)
    (errMsg: string)
    (validate: 'a -> CustomResult<'a>)
    : ValidationResult<'a> =
    match value with
    | Some v ->
        match validate v with
        | Ok r -> ((fieldName, value), Ok r)
        | Error err -> ((fieldName, value), Error err)
    | None -> ((fieldName, value), (Error [ errMsg ]))

let nonEmptyString (fieldName: string, value: string option) =
    let errMsg = sprintf "O campo %s não pode ser vazio" fieldName

    let validate v =
        if String.IsNullOrWhiteSpace v then
            Error [ errMsg ]
        else
            Ok v

    runValidation (fieldName, value) errMsg validate



let isStringLen len (fieldName, value) =
    let errMsg = sprintf "O campo %s deve ter %i caracteres" fieldName len

    let validate v =
        if String.length v <> len then Error([ errMsg ]) else Ok v

    runValidation (fieldName, value) errMsg validate

let isBetween min max (fieldName, value) =

    if value ><! (min, max) then
        Error(sprintf "O valor do campo %s deve estar entre %i e %i" fieldName min max)
    else
        Ok value

let hasMaximumStrLen max (fieldName, value) =
    let errMsg = sprintf "O campo %s deve ter no máximo %i caracteres" fieldName max

    let validate v =
        if (String.length v > max) then Error([ errMsg ]) else Ok v

    runValidation (fieldName, value) errMsg validate

let isStringLenBetween min max (fieldName, value) =
    let strgLen = String.length value

    if strgLen ><! (min, max) then
        Error(sprintf "O campo %s deve ter entre %i e %i carácteres" fieldName min max)
    else
        Ok value


let ufs =
    [| "AC"
       "AL"
       "AP"
       "AM"
       "BA"
       "BA"
       "CE"
       "DF"
       "ES"
       "GO"
       "MA"
       "MT"
       "MS"
       "MG"
       "PA"
       "PB"
       "PR"
       "PE"
       "PI"
       "RJ"
       "RN"
       "RS"
       "RO"
       "RR"
       "SC"
       "SP"
       "SE"
       "TO" |]

let isEstadoValido (fieldName, value) =
    let errMsg = sprintf "O campo %s deve ser uma UF válida" fieldName

    let validate uf =
        if (Array.contains uf ufs) then Ok uf else Error([ errMsg ])

    runValidation (fieldName, value) errMsg validate




let validarCpf (cpf: string) =
    let cpfRegex = Regex(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$")

    if cpfRegex.IsMatch(cpf) then
        Ok cpf
    else
        Error "Cpf inválido"


let validarCnpj (cnpj: string) =
    let cnpjNumeros = Regex.Replace(cnpj, "\\D", "")

    match cnpjNumeros with
    | s when String.length s <> 14 -> Error "CNPJ inválido"
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
            Ok cnpjNumeros
        else
            Error "CNPJ inválido"
