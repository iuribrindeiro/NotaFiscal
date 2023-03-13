module Validation

open RequestTypes
open NotaFiscalServico
open ValidationUtils
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Validation.Validation
open System



let validaCriacaoNotaFiscisRequired = Ok None

let validateEmptyString (value, fieldName) =
    if String.IsNullOrWhiteSpace value then
        Error [ sprintf "O campo %s não pode ser vazio" fieldName ]
    else
        Ok value

let validateStringNotEqualTo compare (value, fieldName) =
    if value = compare then
        Error [ sprintf "O campo %s não pode ser igual a %s" fieldName compare ]
    else
        Ok value

let value = Some "Test"


(value, "Rua") Validation.bind validateEmptyString Validation.bind validateStringNotEqualTo "Test"


//let validaTomadorPessoaFiisRequiredessoaFisico =
let validarEndereco (endereco: EnderecoViewModel) =
    let validarRua = requiredField "Rua" (hasMaximumStrLen 120)

    let validarNumero = requiredField "Numero" (hasMaximumStrLen 10)

    let validarComplemento = optionalStrField "Complemento" (hasMaximumStrLen 60)

    let validarBairro = requiredField "Bairro" (hasMaximumStrLen 60)

    let validarCodigoMunicipio = requiredField "Código do Município" (isStringLen 7)

    let validarCep = requiredField "CEP" (isStringLen 8)

    let validarUf value =
        let fieldName = "UF"
        let strUf = requiredField fieldName (isEstadoValido) value

        match strUf with
        | Ok uf -> validOption (fieldName, ufFromString uf)
        | Error err -> Error err



    validation {
        let! rua = validarRua endereco.Rua
        let! numero = validarNumero endereco.Numero
        let! complemento = validarComplemento endereco.Complemento
        let! bairro = validarBairro endereco.Bairro
        let! codigoMunicio = validarCodigoMunicipio endereco.CodigoMunicipio
        let! cep = validarCep endereco.Cep
        let! uf = validarUf endereco.UF


        return
            { Rua = rua
              Numero = numero
              Complemento = complemento
              Bairro = bairro
              CodigoMunicipio = codigoMunicio
              UF = uf
              Cep = cep }
    }
