module NotaFiscal.Data.Mappers.TomadorMapper

open NotaFiscal.Domain.ApplicationErrors
open NotaFiscal.Data.DbAutoGen
open NotaFiscal.Domain.NotaFiscalPrimitives
open NotaFiscal.Domain.Tomador
open NotaFiscal.Domain.Endereco
open SqlHydra.Query



let toEnderecoDomain createEndereco (endereco: dbo.Endereco) =
    createEndereco
    <| endereco.Id
    <| endereco.Rua
    <| endereco.Numero
    <| endereco.Complemento
    <| endereco.Bairro
    <| endereco.CodigoMunicipio
    <| endereco.Cep

let toContatoDomain createContato telefone email = createContato telefone email

let toTomadorFisicaDomain id cpf inscricaoMunicipal (enderecoDb: dbo.Endereco option) (contatoDb: dbo.Contato option) =
    let contato =
        match contatoDb with
        | Some c ->
            let contatoResult =
                toContatoDomain createContatoPessoaFisica c.Id c.Telefone c.Email
            Some <!> contatoResult
        | None -> succeed None

    let endereco =
        match enderecoDb with
        | Some e ->
            let enderecoResult =
                toEnderecoDomain createEnderecoPessoaFisica e

            Some <!> enderecoResult
        | None -> succeed None

    createTomadorPessoaFisica
    <| id
    <| createCpf cpf
    <| createInscricaoMunicipalTomadorFisico inscricaoMunicipal
    <| endereco
    <| contato

let toTomadorJuridicaDomain
    id
    cnpj
    inscricaoMunicipal
    razaoSocial
    (enderecoDb: dbo.Endereco option)
    (contatoDb: dbo.Contato option)
    =
    let endereco =
        match enderecoDb with
        | Some e -> toEnderecoDomain createEnderecoPessoaJuridica e
        | None -> fail FailConvertEnderecoFromDb

    let contato =
        match contatoDb with
        | Some c -> toContatoDomain createContatoPessoaJuridica c.Id c.Telefone c.Email
        | None -> fail FailConvertContatoFromDb

    createTomadorPessoaJuridica
    <| id
    <| createCnpjTomador cnpj
    <| createInscricaoMunicipalTomadorJuridico inscricaoMunicipal
    <| createRazaoSocial (razaoSocial |> Option.defaultValue "")
    <| endereco
    <| contato

let toTomadorDomain
    (tomadorDb: dbo.Tomador option)
    (enderecoDb: dbo.Endereco option)
    (contatoDb: dbo.Contato option)
    (tomadorDiscriminator)
    =
    match (tomadorDiscriminator, tomadorDb) with
    | nameof (PessoaFisica), None -> PessoaFisica None |> succeed
    | nameof (PessoaFisica), Some tomador ->
        toTomadorFisicaDomain tomador.Id tomador.CpfCnpj tomador.InscricaoMunicipal enderecoDb contatoDb
    | nameof (PessoaJuridica), Some tomador ->
        toTomadorJuridicaDomain
            tomador.Id
            tomador.CpfCnpj
            tomador.InscricaoMunicipal
            tomador.RazaoSocial
            enderecoDb
            contatoDb
    | nameof (Estrangeiro), None -> succeed tomadorEstrangeiro
    | _ -> fail FailToConvertTomadorFromDbError


let toTomadorDiscriminatorDb (tomador: Tomador) =
    match tomador with
    | PessoaFisica _ -> nameof (PessoaFisica)
    | PessoaJuridica _ -> nameof (PessoaJuridica)
    | Estrangeiro _ -> nameof (Estrangeiro)

let toContatoDb (contato: Contato option) =
    match contato with
    | Some c ->
        { dbo.Contato.Id = c.Id
          dbo.Contato.Telefone = c.Telefone |> Telefone.mapValue
          dbo.Contato.Email = c.Email |> EmailAddress.mapToValue }
        |> Some
    | None -> None

let toEnderecoDb (endereco: Endereco option) =
    match endereco with
    | Some e ->
        { dbo.Endereco.Id = e.Id
          dbo.Endereco.Rua = e.Rua |> StrMax120.mapToValue
          dbo.Endereco.Numero = e.Numero |> StrMax60.mapToValue
          dbo.Endereco.Complemento = e.Complemento |> StrMax60.mapToValueOptional
          dbo.Endereco.Bairro = e.Bairro |> StrMax60.mapToValue
          dbo.Endereco.CodigoMunicipio = e.CodigoMunicipio |> StrMax7.mapToValue
          dbo.Endereco.Cep = e.Cep |> StrOf8.mapToValue }
        |> Some
    | None -> None

let toTomadorDb (tomador: Tomador) =

    match tomador with
    | PessoaFisica(Some t) ->

        ({ dbo.Tomador.CpfCnpj = t.Cpf |> CPF.mapToValue
           dbo.Tomador.InscricaoMunicipal = t.InscricaoMunicipal |> StrMax15.mapToValueOptional
           dbo.Tomador.RazaoSocial = None
           dbo.Tomador.Id = t.Id
           dbo.Tomador.ContatoId = t.Contato |> Option.map (fun x -> x.Id)
           dbo.Tomador.EnderecoId = t.Endereco |> Option.map (fun x -> x.Id) }
         |> Some,
         t.Contato |> toContatoDb,
         t.Endereco |> toEnderecoDb)
    | PessoaJuridica t ->
        ({ dbo.Tomador.CpfCnpj = t.Cnpj |> CNPJ.mapToValue
           dbo.Tomador.InscricaoMunicipal = t.InscricaoMunicipal |> StrMax15.mapToValueOptional
           dbo.Tomador.RazaoSocial = (StrMax115.mapToValue t.RazaoSocial) |> Some
           dbo.Tomador.Id = t.Id
           dbo.Tomador.ContatoId = Some t.Contato.Id
           dbo.Tomador.EnderecoId = Some t.Endereco.Id }
         |> Some,
         Some t.Contato |> toContatoDb,
         Some t.Endereco |> toEnderecoDb)
    | PessoaFisica None -> (None, None, None)
    | Estrangeiro TomadorEstrangeiro -> (None, None, None)
    
    
let maybeAddContatoAsync ctx (contatoDb: dbo.Contato option) : Async<int> =
    let addContatoAsync' contato =
        insertAsync (Shared ctx) {
            for _ in dbo.Contato do
                entity contato
        }

    match contatoDb with
    | None -> async { return 0 }
    | Some c -> addContatoAsync' c
    
let maybeAddEnderecoAsync ctx (enderecoDb: dbo.Endereco option) : Async<int> =
    let addEnderecoAsync' endereco =
        insertAsync (Shared ctx) {
            for _ in dbo.Endereco do
                entity endereco
        }

    match enderecoDb with
    | None -> async { return 0 }
    | Some e -> addEnderecoAsync' e
    
let maybeAddTomadorAsync ctx (tomadorDb: dbo.Tomador option) : Async<int> =
    let addTomadorAsync' tomador =
        insertAsync (Shared ctx) {
            for _ in dbo.Tomador do
                entity tomador
        }

    match tomadorDb with
    | None -> async { return 0 }
    | Some t -> addTomadorAsync' t