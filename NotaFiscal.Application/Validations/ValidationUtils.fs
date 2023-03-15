module NotaFiscal.WebApplication.Validations.ValidationUtils

open System
open System.Text.RegularExpressions
open NotaFiscal.Domain.NotaFiscalServico
open FsToolkit.ErrorHandling
open System.ComponentModel.DataAnnotations

type name = string
type Input<'value> = (name * 'value)
type ValidationResult<'valueInput, 'valueResult> = Result<Input<'valueResult>, Input<'valueInput> * string list>
let mapOkResult (_: string, value: 'a) = value

let mapOptionalResult (input: Input<'value> option) =
    match input with
    | Some(_, value) -> Some value
    | None -> None

let emptyFieldErrorMsg name =
    sprintf "O campo %s não pode ser vázio" name

let field (name: string) (value: 'a) : string * 'a = (name, value)

let validateOptional (value: 'a option) (validation: 'a -> Result<'b, 'c>) =
    value |> Option.traverseResult validation

let isNotEmptyInt (name, value) =
    if (value = 0) then
        Error((name, value), [ emptyFieldErrorMsg name ])
    else
        Ok(name, value)

let isEmptyString = String.IsNullOrWhiteSpace
let isNotEmptyStr value = not (isEmptyString value)

let isNotEmptyString (name: string, value: string) =

    if isEmptyString value then
        Error((name, value), [ emptyFieldErrorMsg name ])
    else
        Ok(name, value)

let hasMaxLen (len: int) (name: string, value: string) : ValidationResult<string, string> =
    let errMsg = sprintf "O campo %s deve ter no máximo %d caracteres" name len

    if (isNotEmptyStr value) && String.length value > len then
        Error((name, value), [ errMsg ])
    else
        Ok(name, value)

let hasStringLen (len: int) (name: string, value: string) : ValidationResult<string, string> =
    let errMsg = sprintf "O campo %s deve ter %d carácteres" name len

    if String.length value <> len then
        Error((name, value), [ errMsg ])
    else
        Ok(name, value)

let isUf (name: string, value: string) : ValidationResult<string, UF> =
    let errMsg = sprintf "O campo %s deve ser uma UF válida" name

    match ufFromString value with
    | Some uf -> Ok(name, uf)
    | None -> Error((name, value), [ errMsg ])


let isValidEmail (name: string, value: string) =
    let emailRegex = Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")

    if emailRegex.IsMatch value then
        Ok(name, value)
    else
        Error((name, value), [ sprintf "O %s informado não é válido" name ])


let isValidCpf (name: string, value: string) =
    let cpfRegex = Regex(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$")

    if cpfRegex.IsMatch value then
        Ok(name, value)
    else
        Error((name, value), [ sprintf "O %s informado não é válido" name ])


let isValidCnpj (name: string, value: string) : ValidationResult<string, string> =
    let cnpjNumeros = Regex.Replace(value, "\\D", "")

    let error = (Error((name, value), [ sprintf "O %s informado não é válido" name ]))

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
            Ok(name, value)
        else
            error
