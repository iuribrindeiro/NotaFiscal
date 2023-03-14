module NotaFiscal.WebApplication.Validations.Endereco.EnderecoValidation

open NotaFiscal.Domain.NotaFiscalServico
open NotaFiscal.WebApplication.Validations.ValidationUtils
open NotaFiscal.WebApplication.Validations.ValidationOperators
open FsToolkit.ErrorHandling
open RequestTypes

let validaCriacaoNotaFiscisRequired = Ok None

let validarEndereco (enderedoVM: EnderecoViewModel) =
    let validarRua value =
        field "Rua" value |> isNotEmptyString >>= hasMaxLen 120

    let validarNumero value =
        field "Numero" value |> isNotEmptyString >>= hasMaxLen 60

    let validarComplemento value =
        field "Complemento" value |> hasMaxLen 60

    let validarBairro (value: string) = field "Bairro" value |> (hasMaxLen 60)

    let validarCodigoMunicipio value =
        field "Código do Município" value |> isNotEmptyString >>= (hasStringLen 7)

    let validarCep value =
        field "CEP" value |> isNotEmptyString >>= hasStringLen 8

    let validarUf value =
        field "UF" value |> isNotEmptyString >>= isUf

    validation {
        let! rua = validarRua enderedoVM.Rua
        and! numero = validarNumero enderedoVM.Numero
        and! bairro = validarBairro enderedoVM.Bairro
        and! codigoMunicio = validarCodigoMunicipio enderedoVM.CodigoMunicipio
        and! cep = validarCep enderedoVM.Cep
        and! validUf = validarUf enderedoVM.UF
        and! complement = enderedoVM.Complemento |> Option.traverseResult validarComplemento

        let endereco: Endereco =
            { Rua = mapOkResult rua
              Numero = mapOkResult numero
              Bairro = mapOkResult bairro
              CodigoMunicipio = mapOkResult codigoMunicio
              Complemento = mapOptionalResult complement
              UF = (mapOkResult validUf)
              Cep = mapOkResult cep }


        return endereco

    }
