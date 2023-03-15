module NotaFiscal.WebApplication.Validations.Tomador.TomadorValidation

open NotaFiscal.WebApplication.RequestTypes
open NotaFiscal.WebApplication.Validations.ValidationUtils
open NotaFiscal.WebApplication.Validations.ValidationOperators
open NotaFiscal.WebApplication.Validations.Endereco.EnderecoValidation
open FsToolkit.ErrorHandling
open NotaFiscal.Domain.NotaFiscalServico

type TomadorResult =
    | DadosTomadorPessoaFisica of DadosTomadorPessoaFisica
    | DadosTomadorPessoaJuridica of DadosTomadorPessoaJuridica

let validarInscricaoMunicipal value =
    field "Inscrição Municipal" value |> hasMaxLen 15


let validarContato (value: ContatoViewModel) =
    let validarTelefone value =
        field "Telefone" value |> isNotEmptyString >>= hasMaxLen 11

    let validarEmail value =
        field "Email" value |> isValidEmail >>= hasMaxLen 80

    validation {
        let! telefone = validarTelefone value.Telefone
        and! email = validarEmail value.Email

        return
            { Telefone = mapOkResult telefone
              Email = mapOkResult email }
    }

let validarPessoaFisica (pessoaFisicaVM: TomadorPessoaFisicaViewModel option) =
    match pessoaFisicaVM with
    | Some pessoaFisica ->
        let validarCpf value =
            (field "CPF" value |> isNotEmptyString) >>= isValidCpf

        validation {
            let! cpf = validarCpf pessoaFisica.Cpf
            and! inscricaoMunicipal = validateOptional pessoaFisica.InscricaoMunicipal validarInscricaoMunicipal
            and! contato = validateOptional pessoaFisica.Contato validarContato
            and! endereco = validateOptional pessoaFisica.Endereco validarEndereco


            let tomadorPessoaFisica: DadosTomadorPessoaFisica =
                { Cpf = mapOkResult cpf
                  InscricaoMunicipal = mapOptionalResult inscricaoMunicipal
                  Contato = contato
                  Endereco = endereco }

            return PessoaFisica(Some tomadorPessoaFisica)
        }
    | None -> Ok(PessoaFisica None)

let validarPessoaJuridica (pessoaJuridicaVM: TomadorPessoaJuridicaViewModel) =
    let validarCnpj value =
        field "CNPJ" value |> isNotEmptyString >>= isValidCnpj

    let validarRazaoSocial value =
        field "Razão Social" value |> isNotEmptyString >>= hasMaxLen 115

    validation {
        let! cnpj = validarCnpj pessoaJuridicaVM.Cnpj
        and! razaoSocial = validarRazaoSocial pessoaJuridicaVM.RazaoSocial
        and! inscricaoMunicipal = validateOptional pessoaJuridicaVM.InscricaoMunicipal validarInscricaoMunicipal
        and! contato = validarContato pessoaJuridicaVM.Contato
        and! endereco = validarEndereco pessoaJuridicaVM.Endereco

        let tomadorPessoaJuridica: DadosTomadorPessoaJuridica =
            { Cnpj = mapOkResult cnpj
              RazaoSocial = mapOkResult razaoSocial
              InscricaoMunicipal = mapOptionalResult inscricaoMunicipal
              Contato = contato
              Endereco = endereco }

        return PessoaJuridica tomadorPessoaJuridica
    }

let validarTomador (tomadorVM: TomadorViewModel option) =
    match tomadorVM with
    | Some tm ->
        match tm with
        | PessoaFisicaVM pessoaFisica -> validarPessoaFisica pessoaFisica
        | PessoaJuridicaVM pessoaJuridica -> validarPessoaJuridica pessoaJuridica
        | EstrangeiroVM -> Ok(Estrangeiro)

    | None -> Ok(PessoaFisica None)
