module NotaFiscal.WebApplication.Validations.ValidationUtils

open System
open System.Text.RegularExpressions
open NotaFiscal.Domain.NotaFiscalServico

type FieldName = string
type Input<'value> = (FieldName * 'value)
type ValidationResult<'valueInput, 'valueResult> = Result<Input<'valueResult>, Input<'valueInput> * string list>
let mapOkResult (_: string, value: 'a) = value

let mapOptionalResult (input: Input<'value> option) =
    match input with
    | Some(_, value) -> Some value
    | None -> None


let field (fieldName: string) (value: 'a) : string * 'a = (fieldName, value)

let isEmptyString = String.IsNullOrWhiteSpace
let isNotEmptyStr value = not (isEmptyString value)

let isNotEmptyString (fieldName: string, value: string) =
    let errMsg = sprintf "O campo %s não pode ser vázio" fieldName

    if isEmptyString value then
        Error((fieldName, value), [ errMsg ])
    else
        Ok(fieldName, value)

let hasMaxLen (len: int) (fieldName: string, value: string) : ValidationResult<string, string> =
    let errMsg = sprintf "O campo %s deve ter no máximo %d caracteres" fieldName len

    if (isNotEmptyStr value) && String.length value > len then
        Error((fieldName, value), [ errMsg ])
    else
        Ok(fieldName, value)

let hasStringLen (len: int) (fieldName: string, value: string) : ValidationResult<string, string> =
    let errMsg = sprintf "O campo %s deve ter %d carácteres" fieldName len

    if String.length value <> len then
        Error((fieldName, value), [ errMsg ])
    else
        Ok(fieldName, value)

let isUf (fieldName: string, value: string) : ValidationResult<string, UF> =
    let errMsg = sprintf "O campo %s deve ser uma UF válida" fieldName

    match ufFromString value with
    | Some uf -> Ok(fieldName, uf)
    | None -> Error((fieldName, value), [ errMsg ])



let isValidCpf (fieldName: string, value: string) =
    let cpfRegex = Regex(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$")

    if cpfRegex.IsMatch value then
        Ok value
    else
        Error [ sprintf "O %s informado não é válido" fieldName ]


let validarCnpj (fieldName: string, value: string) : ValidationResult<string, string> =
    let cnpjNumeros = Regex.Replace(value, "\\D", "")

    let error =
        (Error((fieldName, value), [ sprintf "O %s informado não é válido" fieldName ]))

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
            Ok(fieldName, value)
        else
            error
