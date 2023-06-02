// This code was generated by `SqlHydra.SqlServer` -- v1.2.1.0.
namespace NotaFiscal.Data.DbAutoGen


[<AutoOpen>]
module ColumnReaders =
    type Column(reader: System.Data.IDataReader, getOrdinal: string -> int, column) =
            member __.Name = column
            member __.IsNull() = getOrdinal column |> reader.IsDBNull
            override __.ToString() = __.Name

    type RequiredColumn<'T, 'Reader when 'Reader :> System.Data.IDataReader>(reader: 'Reader, getOrdinal, getter: int -> 'T, column) =
            inherit Column(reader, getOrdinal, column)
            member __.Read(?alias) = alias |> Option.defaultValue __.Name |> getOrdinal |> getter

    type OptionalColumn<'T, 'Reader when 'Reader :> System.Data.IDataReader>(reader: 'Reader, getOrdinal, getter: int -> 'T, column) =
            inherit Column(reader, getOrdinal, column)
            member __.Read(?alias) = 
                match alias |> Option.defaultValue __.Name |> getOrdinal with
                | o when reader.IsDBNull o -> None
                | o -> Some (getter o)

    type RequiredBinaryColumn<'T, 'Reader when 'Reader :> System.Data.IDataReader>(reader: 'Reader, getOrdinal, getValue: int -> obj, column) =
            inherit Column(reader, getOrdinal, column)
            member __.Read(?alias) = alias |> Option.defaultValue __.Name |> getOrdinal |> getValue :?> byte[]

    type OptionalBinaryColumn<'T, 'Reader when 'Reader :> System.Data.IDataReader>(reader: 'Reader, getOrdinal, getValue: int -> obj, column) =
            inherit Column(reader, getOrdinal, column)
            member __.Read(?alias) = 
                match alias |> Option.defaultValue __.Name |> getOrdinal with
                | o when reader.IsDBNull o -> None
                | o -> Some (getValue o :?> byte[])
            
[<AutoOpen>]
module private DataReaderExtensions =
    type System.Data.IDataReader with
        member reader.GetDateOnly(ordinal: int) = 
            reader.GetDateTime(ordinal) |> System.DateOnly.FromDateTime
    
    type System.Data.Common.DbDataReader with
        member reader.GetTimeOnly(ordinal: int) = 
            reader.GetFieldValue(ordinal) |> System.TimeOnly.FromTimeSpan
        
        

module dbo =
    [<CLIMutable>]
    type Contato =
        { Email: string
          Id: System.Guid
          Telefone: string }

    let Contato = SqlHydra.Query.Table.table<Contato>

    [<CLIMutable>]
    type Endereco =
        { Bairro: string
          Cep: string
          CodigoMunicipio: string
          Complemento: Option<string>
          Id: System.Guid
          Numero: string
          Rua: string }

    let Endereco = SqlHydra.Query.Table.table<Endereco>

    [<CLIMutable>]
    type ErroComunicacao =
        { CodigoErro: string
          Correcao: Option<string>
          Id: System.Guid
          Mensagem: string
          NotaFiscalServicoId: System.Guid }

    let ErroComunicacao = SqlHydra.Query.Table.table<ErroComunicacao>

    [<CLIMutable>]
    type NaturezaOperacao =
        { Descricao: string
          Discriminator: string
          Id: int }

    let NaturezaOperacao = SqlHydra.Query.Table.table<NaturezaOperacao>

    [<CLIMutable>]
    type NotaFiscalServico =
        { CodigoCancelamento: Option<string>
          CodigoCnae: Option<string>
          CodigoTributacaoMunicipio: Option<string>
          [<SqlHydra.ProviderDbType("DateTime")>]
          DataEmissao: Option<System.DateTime>
          Discriminacao: string
          Discriminator: string
          Id: System.Guid
          IncentivadorCultural: bool
          ItemListaServico: string
          MunicipioPrestacaoServico: string
          NumeroLote: Option<string>
          NumeroNota: Option<int>
          NumeroProtocolo: Option<string>
          OptanteSimplesNacional: bool
          RpsNumero: Option<int>
          RpsSerie: Option<string>
          ServicoNaturezaOperacaoId: int
          ServicoRegimeEspecialTributacaoId: int
          TipoRpsId: Option<int>
          TomadorDiscriminator: string
          TomadorId: Option<System.Guid>
          ValoresAliquota: Option<double>
          ValoresCofins: Option<decimal>
          ValoresCsll: Option<decimal>
          ValoresDeducoes: Option<decimal>
          ValoresDescontoCondicionado: Option<decimal>
          ValoresDescontoIncondicionado: Option<decimal>
          ValoresInss: Option<decimal>
          ValoresIr: Option<decimal>
          ValoresIss: decimal
          ValoresIssDiscriminator: string
          ValoresOutrasRetencoes: Option<decimal>
          ValoresPis: Option<decimal>
          ValoresServicos: decimal }

    let NotaFiscalServico = SqlHydra.Query.Table.table<NotaFiscalServico>

    [<CLIMutable>]
    type RegimeEspecialTributacao =
        { Descricao: string
          Discriminator: string
          Id: int }

    let RegimeEspecialTributacao = SqlHydra.Query.Table.table<RegimeEspecialTributacao>

    [<CLIMutable>]
    type TipoRps =
        { Descricao: string
          Discriminator: string
          Id: int }

    let TipoRps = SqlHydra.Query.Table.table<TipoRps>

    [<CLIMutable>]
    type Tomador =
        { ContatoId: Option<System.Guid>
          CpfCnpj: string
          EnderecoId: Option<System.Guid>
          Id: System.Guid
          InscricaoMunicipal: Option<string>
          RazaoSocial: Option<string> }

    let Tomador = SqlHydra.Query.Table.table<Tomador>

    module Readers =
        type ContatoReader(reader: Microsoft.Data.SqlClient.SqlDataReader, getOrdinal) =
            member __.Email = RequiredColumn(reader, getOrdinal, reader.GetString, "Email")
            member __.Id = RequiredColumn(reader, getOrdinal, reader.GetGuid, "Id")
            member __.Telefone = RequiredColumn(reader, getOrdinal, reader.GetString, "Telefone")

            member __.Read() =
                { Contato.Email = __.Email.Read()
                  Id = __.Id.Read()
                  Telefone = __.Telefone.Read() }

            member __.ReadIfNotNull() =
                if __.Id.IsNull() then None else Some(__.Read())

        type EnderecoReader(reader: Microsoft.Data.SqlClient.SqlDataReader, getOrdinal) =
            member __.Bairro = RequiredColumn(reader, getOrdinal, reader.GetString, "Bairro")
            member __.Cep = RequiredColumn(reader, getOrdinal, reader.GetString, "Cep")
            member __.CodigoMunicipio = RequiredColumn(reader, getOrdinal, reader.GetString, "CodigoMunicipio")
            member __.Complemento = OptionalColumn(reader, getOrdinal, reader.GetString, "Complemento")
            member __.Id = RequiredColumn(reader, getOrdinal, reader.GetGuid, "Id")
            member __.Numero = RequiredColumn(reader, getOrdinal, reader.GetString, "Numero")
            member __.Rua = RequiredColumn(reader, getOrdinal, reader.GetString, "Rua")
            member __.Uf = RequiredColumn(reader, getOrdinal, reader.GetString, "Uf")

            member __.Read() =
                { Endereco.Bairro = __.Bairro.Read()
                  Cep = __.Cep.Read()
                  CodigoMunicipio = __.CodigoMunicipio.Read()
                  Complemento = __.Complemento.Read()
                  Id = __.Id.Read()
                  Numero = __.Numero.Read()
                  Rua = __.Rua.Read() }

            member __.ReadIfNotNull() =
                if __.Id.IsNull() then None else Some(__.Read())

        type ErroComunicacaoReader(reader: Microsoft.Data.SqlClient.SqlDataReader, getOrdinal) =
            member __.CodigoErro = RequiredColumn(reader, getOrdinal, reader.GetString, "CodigoErro")
            member __.Correcao = OptionalColumn(reader, getOrdinal, reader.GetString, "Correcao")
            member __.Id = RequiredColumn(reader, getOrdinal, reader.GetGuid, "Id")
            member __.Mensagem = RequiredColumn(reader, getOrdinal, reader.GetString, "Mensagem")
            member __.NotaFiscalServicoId = RequiredColumn(reader, getOrdinal, reader.GetGuid, "NotaFiscalServicoId")

            member __.Read() =
                { ErroComunicacao.CodigoErro = __.CodigoErro.Read()
                  Correcao = __.Correcao.Read()
                  Id = __.Id.Read()
                  Mensagem = __.Mensagem.Read()
                  NotaFiscalServicoId = __.NotaFiscalServicoId.Read() }

            member __.ReadIfNotNull() =
                if __.Id.IsNull() then None else Some(__.Read())

        type NaturezaOperacaoReader(reader: Microsoft.Data.SqlClient.SqlDataReader, getOrdinal) =
            member __.Descricao = RequiredColumn(reader, getOrdinal, reader.GetString, "Descricao")
            member __.Discriminator = RequiredColumn(reader, getOrdinal, reader.GetString, "Discriminator")
            member __.Id = RequiredColumn(reader, getOrdinal, reader.GetInt32, "Id")

            member __.Read() =
                { NaturezaOperacao.Descricao = __.Descricao.Read()
                  Discriminator = __.Discriminator.Read()
                  Id = __.Id.Read() }

            member __.ReadIfNotNull() =
                if __.Id.IsNull() then None else Some(__.Read())

        type NotaFiscalServicoReader(reader: Microsoft.Data.SqlClient.SqlDataReader, getOrdinal) =
            member __.CodigoCancelamento = OptionalColumn(reader, getOrdinal, reader.GetString, "CodigoCancelamento")
            member __.CodigoCnae = OptionalColumn(reader, getOrdinal, reader.GetString, "CodigoCnae")
            member __.CodigoTributacaoMunicipio = OptionalColumn(reader, getOrdinal, reader.GetString, "CodigoTributacaoMunicipio")
            member __.DataEmissao = OptionalColumn(reader, getOrdinal, reader.GetDateTime, "DataEmissao")
            member __.Discriminacao = RequiredColumn(reader, getOrdinal, reader.GetString, "Discriminacao")
            member __.Discriminator = RequiredColumn(reader, getOrdinal, reader.GetString, "Discriminator")
            member __.Id = RequiredColumn(reader, getOrdinal, reader.GetGuid, "Id")
            member __.IncentivadorCultural = RequiredColumn(reader, getOrdinal, reader.GetBoolean, "IncentivadorCultural")
            member __.ItemListaServico = RequiredColumn(reader, getOrdinal, reader.GetString, "ItemListaServico")
            member __.MunicipioPrestacaoServico = RequiredColumn(reader, getOrdinal, reader.GetString, "MunicipioPrestacaoServico")
            member __.NumeroLote = OptionalColumn(reader, getOrdinal, reader.GetString, "NumeroLote")
            member __.NumeroNota = OptionalColumn(reader, getOrdinal, reader.GetInt32, "NumeroNota")
            member __.NumeroProtocolo = OptionalColumn(reader, getOrdinal, reader.GetString, "NumeroProtocolo")
            member __.OptanteSimplesNacional = RequiredColumn(reader, getOrdinal, reader.GetBoolean, "OptanteSimplesNacional")
            member __.PrestadorCnpj = RequiredColumn(reader, getOrdinal, reader.GetString, "PrestadorCnpj")
            member __.PrestadorInscricaoMunicipal = RequiredColumn(reader, getOrdinal, reader.GetString, "PrestadorInscricaoMunicipal")
            member __.RpsNumero = OptionalColumn(reader, getOrdinal, reader.GetInt32, "RpsNumero")
            member __.RpsSerie = OptionalColumn(reader, getOrdinal, reader.GetString, "RpsSerie")
            member __.ServicoNaturezaOperacaoId = RequiredColumn(reader, getOrdinal, reader.GetInt32, "ServicoNaturezaOperacaoId")
            member __.ServicoRegimeEspecialTributacaoId = RequiredColumn(reader, getOrdinal, reader.GetInt32, "ServicoRegimeEspecialTributacaoId")
            member __.TipoRpsId = OptionalColumn(reader, getOrdinal, reader.GetInt32, "TipoRpsId")
            member __.TomadorDiscriminator = RequiredColumn(reader, getOrdinal, reader.GetString, "TomadorDiscriminator")
            member __.TomadorId = OptionalColumn(reader, getOrdinal, reader.GetGuid, "TomadorId")
            member __.ValoresAliquota = OptionalColumn(reader, getOrdinal, reader.GetDouble, "ValoresAliquota")
            member __.ValoresCofins = OptionalColumn(reader, getOrdinal, reader.GetDecimal, "ValoresCofins")
            member __.ValoresCsll = OptionalColumn(reader, getOrdinal, reader.GetDecimal, "ValoresCsll")
            member __.ValoresDeducoes = OptionalColumn(reader, getOrdinal, reader.GetDecimal, "ValoresDeducoes")
            member __.ValoresDescontoCondicionado = OptionalColumn(reader, getOrdinal, reader.GetDecimal, "ValoresDescontoCondicionado")
            member __.ValoresDescontoIncondicionado = OptionalColumn(reader, getOrdinal, reader.GetDecimal, "ValoresDescontoIncondicionado")
            member __.ValoresInss = OptionalColumn(reader, getOrdinal, reader.GetDecimal, "ValoresInss")
            member __.ValoresIr = OptionalColumn(reader, getOrdinal, reader.GetDecimal, "ValoresIr")
            member __.ValoresIss = RequiredColumn(reader, getOrdinal, reader.GetDecimal, "ValoresIss")
            member __.ValoresIssDiscriminator = RequiredColumn(reader, getOrdinal, reader.GetString, "ValoresIssDiscriminator")
            member __.ValoresOutrasRetencoes = OptionalColumn(reader, getOrdinal, reader.GetDecimal, "ValoresOutrasRetencoes")
            member __.ValoresPis = OptionalColumn(reader, getOrdinal, reader.GetDecimal, "ValoresPis")
            member __.ValoresServicos = RequiredColumn(reader, getOrdinal, reader.GetDecimal, "ValoresServicos")

            member __.Read() =
                { NotaFiscalServico.CodigoCancelamento = __.CodigoCancelamento.Read()
                  CodigoCnae = __.CodigoCnae.Read()
                  CodigoTributacaoMunicipio = __.CodigoTributacaoMunicipio.Read()
                  DataEmissao = __.DataEmissao.Read()
                  Discriminacao = __.Discriminacao.Read()
                  Discriminator = __.Discriminator.Read()
                  Id = __.Id.Read()
                  IncentivadorCultural = __.IncentivadorCultural.Read()
                  ItemListaServico = __.ItemListaServico.Read()
                  MunicipioPrestacaoServico = __.MunicipioPrestacaoServico.Read()
                  NumeroLote = __.NumeroLote.Read()
                  NumeroNota = __.NumeroNota.Read()
                  NumeroProtocolo = __.NumeroProtocolo.Read()
                  OptanteSimplesNacional = __.OptanteSimplesNacional.Read()
                  RpsNumero = __.RpsNumero.Read()
                  RpsSerie = __.RpsSerie.Read()
                  ServicoNaturezaOperacaoId = __.ServicoNaturezaOperacaoId.Read()
                  ServicoRegimeEspecialTributacaoId = __.ServicoRegimeEspecialTributacaoId.Read()
                  TipoRpsId = __.TipoRpsId.Read()
                  TomadorDiscriminator = __.TomadorDiscriminator.Read()
                  TomadorId = __.TomadorId.Read()
                  ValoresAliquota = __.ValoresAliquota.Read()
                  ValoresCofins = __.ValoresCofins.Read()
                  ValoresCsll = __.ValoresCsll.Read()
                  ValoresDeducoes = __.ValoresDeducoes.Read()
                  ValoresDescontoCondicionado = __.ValoresDescontoCondicionado.Read()
                  ValoresDescontoIncondicionado = __.ValoresDescontoIncondicionado.Read()
                  ValoresInss = __.ValoresInss.Read()
                  ValoresIr = __.ValoresIr.Read()
                  ValoresIss = __.ValoresIss.Read()
                  ValoresIssDiscriminator = __.ValoresIssDiscriminator.Read()
                  ValoresOutrasRetencoes = __.ValoresOutrasRetencoes.Read()
                  ValoresPis = __.ValoresPis.Read()
                  ValoresServicos = __.ValoresServicos.Read() }

            member __.ReadIfNotNull() =
                if __.Id.IsNull() then None else Some(__.Read())

        type RegimeEspecialTributacaoReader(reader: Microsoft.Data.SqlClient.SqlDataReader, getOrdinal) =
            member __.Descricao = RequiredColumn(reader, getOrdinal, reader.GetString, "Descricao")
            member __.Discriminator = RequiredColumn(reader, getOrdinal, reader.GetString, "Discriminator")
            member __.Id = RequiredColumn(reader, getOrdinal, reader.GetInt32, "Id")

            member __.Read() =
                { RegimeEspecialTributacao.Descricao = __.Descricao.Read()
                  Discriminator = __.Discriminator.Read()
                  Id = __.Id.Read() }

            member __.ReadIfNotNull() =
                if __.Id.IsNull() then None else Some(__.Read())

        type TipoRpsReader(reader: Microsoft.Data.SqlClient.SqlDataReader, getOrdinal) =
            member __.Descricao = RequiredColumn(reader, getOrdinal, reader.GetString, "Descricao")
            member __.Discriminator = RequiredColumn(reader, getOrdinal, reader.GetString, "Discriminator")
            member __.Id = RequiredColumn(reader, getOrdinal, reader.GetInt32, "Id")

            member __.Read() =
                { TipoRps.Descricao = __.Descricao.Read()
                  Discriminator = __.Discriminator.Read()
                  Id = __.Id.Read() }

            member __.ReadIfNotNull() =
                if __.Id.IsNull() then None else Some(__.Read())

        type TomadorReader(reader: Microsoft.Data.SqlClient.SqlDataReader, getOrdinal) =
            member __.ContatoId = OptionalColumn(reader, getOrdinal, reader.GetGuid, "ContatoId")
            member __.CpfCnpj = RequiredColumn(reader, getOrdinal, reader.GetString, "CpfCnpj")
            member __.EnderecoId = OptionalColumn(reader, getOrdinal, reader.GetGuid, "EnderecoId")
            member __.Id = RequiredColumn(reader, getOrdinal, reader.GetGuid, "Id")
            member __.InscricaoMunicipal = OptionalColumn(reader, getOrdinal, reader.GetString, "InscricaoMunicipal")
            member __.RazaoSocial = OptionalColumn(reader, getOrdinal, reader.GetString, "RazaoSocial")

            member __.Read() =
                { Tomador.ContatoId = __.ContatoId.Read()
                  CpfCnpj = __.CpfCnpj.Read()
                  EnderecoId = __.EnderecoId.Read()
                  Id = __.Id.Read()
                  InscricaoMunicipal = __.InscricaoMunicipal.Read()
                  RazaoSocial = __.RazaoSocial.Read() }

            member __.ReadIfNotNull() =
                if __.Id.IsNull() then None else Some(__.Read())

type HydraReader(reader: Microsoft.Data.SqlClient.SqlDataReader) =
    let mutable accFieldCount = 0
    let buildGetOrdinal fieldCount =
        let dictionary = 
            [0..reader.FieldCount-1] 
            |> List.map (fun i -> reader.GetName(i), i)
            |> List.sortBy snd
            |> List.skip accFieldCount
            |> List.take fieldCount
            |> dict
        accFieldCount <- accFieldCount + fieldCount
        fun col -> dictionary.Item col
        
    let lazydboContato = lazy (dbo.Readers.ContatoReader(reader, buildGetOrdinal 3))
    let lazydboEndereco = lazy (dbo.Readers.EnderecoReader(reader, buildGetOrdinal 8))
    let lazydboErroComunicacao = lazy (dbo.Readers.ErroComunicacaoReader(reader, buildGetOrdinal 5))
    let lazydboNaturezaOperacao = lazy (dbo.Readers.NaturezaOperacaoReader(reader, buildGetOrdinal 3))
    let lazydboNotaFiscalServico = lazy (dbo.Readers.NotaFiscalServicoReader(reader, buildGetOrdinal 38))
    let lazydboRegimeEspecialTributacao = lazy (dbo.Readers.RegimeEspecialTributacaoReader(reader, buildGetOrdinal 3))
    let lazydboTipoRps = lazy (dbo.Readers.TipoRpsReader(reader, buildGetOrdinal 3))
    let lazydboTomador = lazy (dbo.Readers.TomadorReader(reader, buildGetOrdinal 6))
    member __.``dbo.Contato`` = lazydboContato.Value
    member __.``dbo.Endereco`` = lazydboEndereco.Value
    member __.``dbo.ErroComunicacao`` = lazydboErroComunicacao.Value
    member __.``dbo.NaturezaOperacao`` = lazydboNaturezaOperacao.Value
    member __.``dbo.NotaFiscalServico`` = lazydboNotaFiscalServico.Value
    member __.``dbo.RegimeEspecialTributacao`` = lazydboRegimeEspecialTributacao.Value
    member __.``dbo.TipoRps`` = lazydboTipoRps.Value
    member __.``dbo.Tomador`` = lazydboTomador.Value
    member private __.AccFieldCount with get () = accFieldCount and set (value) = accFieldCount <- value

    member private __.GetReaderByName(entity: string, isOption: bool) =
        match entity, isOption with
        | "dbo.Contato", false -> __.``dbo.Contato``.Read >> box
        | "dbo.Contato", true -> __.``dbo.Contato``.ReadIfNotNull >> box
        | "dbo.Endereco", false -> __.``dbo.Endereco``.Read >> box
        | "dbo.Endereco", true -> __.``dbo.Endereco``.ReadIfNotNull >> box
        | "dbo.ErroComunicacao", false -> __.``dbo.ErroComunicacao``.Read >> box
        | "dbo.ErroComunicacao", true -> __.``dbo.ErroComunicacao``.ReadIfNotNull >> box
        | "dbo.NaturezaOperacao", false -> __.``dbo.NaturezaOperacao``.Read >> box
        | "dbo.NaturezaOperacao", true -> __.``dbo.NaturezaOperacao``.ReadIfNotNull >> box
        | "dbo.NotaFiscalServico", false -> __.``dbo.NotaFiscalServico``.Read >> box
        | "dbo.NotaFiscalServico", true -> __.``dbo.NotaFiscalServico``.ReadIfNotNull >> box
        | "dbo.RegimeEspecialTributacao", false -> __.``dbo.RegimeEspecialTributacao``.Read >> box
        | "dbo.RegimeEspecialTributacao", true -> __.``dbo.RegimeEspecialTributacao``.ReadIfNotNull >> box
        | "dbo.TipoRps", false -> __.``dbo.TipoRps``.Read >> box
        | "dbo.TipoRps", true -> __.``dbo.TipoRps``.ReadIfNotNull >> box
        | "dbo.Tomador", false -> __.``dbo.Tomador``.Read >> box
        | "dbo.Tomador", true -> __.``dbo.Tomador``.ReadIfNotNull >> box
        | _ -> failwith $"Could not read type '{entity}' because no generated reader exists."

    static member private GetPrimitiveReader(t: System.Type, reader: Microsoft.Data.SqlClient.SqlDataReader, isOpt: bool) =
        let wrap get (ord: int) = 
            if isOpt 
            then (if reader.IsDBNull ord then None else get ord |> Some) |> box 
            else get ord |> box 
        

        if t = typedefof<System.Guid> then Some(wrap reader.GetGuid)
        else if t = typedefof<bool> then Some(wrap reader.GetBoolean)
        else if t = typedefof<int> then Some(wrap reader.GetInt32)
        else if t = typedefof<int64> then Some(wrap reader.GetInt64)
        else if t = typedefof<int16> then Some(wrap reader.GetInt16)
        else if t = typedefof<byte> then Some(wrap reader.GetByte)
        else if t = typedefof<double> then Some(wrap reader.GetDouble)
        else if t = typedefof<System.Single> then Some(wrap reader.GetFloat)
        else if t = typedefof<decimal> then Some(wrap reader.GetDecimal)
        else if t = typedefof<string> then Some(wrap reader.GetString)
        else if t = typedefof<System.DateTimeOffset> then Some(wrap reader.GetDateTimeOffset)
        else if t = typedefof<System.DateOnly> then Some(wrap reader.GetDateOnly)
        else if t = typedefof<System.TimeOnly> then Some(wrap reader.GetTimeOnly)
        else if t = typedefof<System.DateTime> then Some(wrap reader.GetDateTime)
        else if t = typedefof<byte []> then Some(wrap reader.GetValue)
        else if t = typedefof<obj> then Some(wrap reader.GetValue)
        else None

    static member Read(reader: Microsoft.Data.SqlClient.SqlDataReader) = 
        let hydra = HydraReader(reader)
                    
        let getOrdinalAndIncrement() = 
            let ordinal = hydra.AccFieldCount
            hydra.AccFieldCount <- hydra.AccFieldCount + 1
            ordinal
            
        let buildEntityReadFn (t: System.Type) = 
            let t, isOpt = 
                if t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<Option<_>> 
                then t.GenericTypeArguments.[0], true
                else t, false
            
            match HydraReader.GetPrimitiveReader(t, reader, isOpt) with
            | Some primitiveReader -> 
                let ord = getOrdinalAndIncrement()
                fun () -> primitiveReader ord
            | None ->
                let nameParts = t.FullName.Split([| '.'; '+' |])
                let schemaAndType = nameParts |> Array.skip (nameParts.Length - 2) |> fun parts -> System.String.Join(".", parts)
                hydra.GetReaderByName(schemaAndType, isOpt)
            
        // Return a fn that will hydrate 'T (which may be a tuple)
        // This fn will be called once per each record returned by the data reader.
        let t = typeof<'T>
        if FSharp.Reflection.FSharpType.IsTuple(t) then
            let readEntityFns = FSharp.Reflection.FSharpType.GetTupleElements(t) |> Array.map buildEntityReadFn
            fun () ->
                let entities = readEntityFns |> Array.map (fun read -> read())
                Microsoft.FSharp.Reflection.FSharpValue.MakeTuple(entities, t) :?> 'T
        else
            let readEntityFn = t |> buildEntityReadFn
            fun () -> 
                readEntityFn() :?> 'T
        
