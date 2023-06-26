module NotaFiscal.Domain.Dto.TomadorDto

open System
open NotaFiscal.Domain.Endereco
open NotaFiscal.Domain.NotaFiscalPrimitives
open NotaFiscal.Domain.DomainEvents
open NotaFiscal.Domain.Tomador
open NotaFiscal.Domain.Rop

type TomadorDto =
    { Discriminator: string
      Cpf: string
      Cnpj: string
      InscricaoMunicipal: string
      Nome: string
      Endereco: EnderecoDto
      Contato: ContatoDto }

and EnderecoDto =
    { Rua: string
      Numero: string
      Complemento: string
      Bairro: string
      CodigoMunicipio: string
      Cep: string }

and ContatoDto = { Telefone: string; Email: string }

let toLowerInvariant (s: string) = if String.IsNullOrWhiteSpace s then s else s.ToLowerInvariant()

let mapEnderecoFromDomain (endereco: Endereco) =
    { Rua = endereco.Rua |> StrMax120.mapToValue
      Numero = endereco.Numero |> StrMax60.mapToValue
      Complemento = StrMax60.mapToValueOptional endereco.Complemento |> Option.defaultValue null
      Bairro = endereco.Bairro |> StrMax60.mapToValue
      CodigoMunicipio = endereco.CodigoMunicipio |> StrMax7.mapToValue
      Cep = endereco.Cep |> StrOf8.mapToValue }

let mapContatoFromDomain (contato: Contato) =
    { Email = contato.Email |> EmailAddress.mapToValue
      Telefone = contato.Telefone |> Telefone.mapValue }

let fromTomadorPessoaFisica (tomador: TomadorPessoaFisica option) =
    match tomador with
    | Some pf ->
        { Discriminator = nameof PessoaFisica
          Cpf = CPF.mapToValue pf.Cpf
          Nome = StrMax115.mapToValueOptional pf.Nome |> Option.defaultValue null
          Endereco = Option.map mapEnderecoFromDomain pf.Endereco |> Option.defaultValue Unchecked.defaultof<EnderecoDto>
          Contato = Option.map mapContatoFromDomain pf.Contato |> Option.defaultValue Unchecked.defaultof<ContatoDto>
          Cnpj = null
          InscricaoMunicipal = null }
    | None ->
        { Discriminator = nameof PessoaFisica
          Cnpj = null
          Cpf = null
          InscricaoMunicipal = null
          Nome = null
          Endereco = Unchecked.defaultof<EnderecoDto>
          Contato = Unchecked.defaultof<ContatoDto> }

let fromTomadorPessoaJuridica (tomador: TomadorPessoaJuridica) =
    { Discriminator = nameof PessoaJuridica
      Cnpj = CNPJ.mapToValue tomador.Cnpj
      Cpf = null
      InscricaoMunicipal = StrMax15.mapToValueOptional tomador.InscricaoMunicipal |> Option.defaultValue null
      Nome = StrMax115.mapToValue tomador.RazaoSocial
      Endereco = mapEnderecoFromDomain tomador.Endereco
      Contato = mapContatoFromDomain tomador.Contato }

let fromTomadorEstrangeiro (tomador: TomadorEstrangeiro) =
    { Discriminator = nameof Estrangeiro
      Cnpj = null
      Cpf = null
      InscricaoMunicipal = null
      Nome = StrMax115.mapToValueOptional tomador.Nome
             |> Option.defaultValue null
      Endereco = Option.map mapEnderecoFromDomain tomador.Endereco
                 |> Option.defaultValue Unchecked.defaultof<EnderecoDto>
      Contato = Option.map mapContatoFromDomain tomador.Contato
                |> Option.defaultValue Unchecked.defaultof<ContatoDto> }

let fromTomadorDomain (tomador: Tomador) : TomadorDto =
    match tomador with
    | PessoaFisica t -> fromTomadorPessoaFisica t
    | PessoaJuridica pj -> fromTomadorPessoaJuridica pj
    | Estrangeiro es -> fromTomadorEstrangeiro es
    

let toTomadorDomain (tomadorDto: TomadorDto) =
    let createEndereco enderecoDto f =
        f
            <| enderecoDto.Rua
            <| enderecoDto.Numero
            <| Some enderecoDto.Complemento
            <| enderecoDto.Bairro
            <| enderecoDto.CodigoMunicipio
            <| enderecoDto.Cep
        
    let createContato contatoDto f =
        f contatoDto.Telefone contatoDto.Email
    
    let toEnderecoPessoaFisicaDomain (enderecoDto: EnderecoDto) =
        createEndereco enderecoDto createEnderecoPessoaFisica
        |> mapR Some
        
    let toEnderecoPessoaJuridicaDomain (enderecoDto: EnderecoDto) =
        createEndereco enderecoDto createEnderecoPessoaJuridica
    
    let toContatoPessoaFisicaDomain (contatoDto: ContatoDto) =
        createContato contatoDto createContatoPessoaFisica
        |> mapR Some
        
    let toContatoPessoaJuridicaDomain (contatoDto: ContatoDto) =
        createContato contatoDto createContatoPessoaJuridica
        
    let toEnderedoTomadorEstrangeiroDomain (enderecoDto: EnderecoDto) =
        createEndereco enderecoDto createEnderecoTomadorEstrangeiro
        |> mapR Some
        
    let toContatoTomadorEstrangeiroDomain (contatoDto: ContatoDto) =
        createContato contatoDto createContatoTomadorEstrangeiro
        |> mapR Some
              
    match box tomadorDto with
    | null -> TomadorIsRequired |> fail
    | _ when tomadorDto.Discriminator |> toLowerInvariant = (nameof PessoaFisica |> toLowerInvariant) ->
        createTomadorPessoaFisica
            <| tomadorDto.Cpf
            <| Some tomadorDto.InscricaoMunicipal
            <| Some tomadorDto.Nome
            <*> mapNullToOptR tomadorDto.Endereco toEnderecoPessoaFisicaDomain
            <*> mapNullToOptR tomadorDto.Contato toContatoPessoaFisicaDomain
    | _ when tomadorDto.Discriminator |> toLowerInvariant = (nameof PessoaJuridica |> toLowerInvariant) ->
        createTomadorPessoaJuridica
            <| tomadorDto.Cnpj
            <| Some tomadorDto.InscricaoMunicipal
            <| tomadorDto.Nome
        <*> mapNullToR tomadorDto.Contato toContatoPessoaJuridicaDomain (ContatoIsRequired |> PessoaJuridicaInvalida |> TomadorInvalido)
        <*> mapNullToR tomadorDto.Endereco toEnderecoPessoaJuridicaDomain (EnderecoIsRequired |> PessoaJuridicaInvalida |> TomadorInvalido)
    | _ when tomadorDto.Discriminator |> toLowerInvariant = (nameof Estrangeiro |> toLowerInvariant) ->
        createTomadorEstrangeiro
            <| Some tomadorDto.Nome
        <*> mapNullToOptR tomadorDto.Endereco toEnderedoTomadorEstrangeiroDomain
        <*> mapNullToOptR tomadorDto.Contato toContatoTomadorEstrangeiroDomain
    | _ -> PessoaFisica None |> succeed


